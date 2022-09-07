using Imdb.Infrastructure.Entities;
using Imdb.Infrastructure.IRepository;
using Imdb.Infrastructure.Repository;
using Infrastructure.Repositories.IRepository;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shared.Exceptions;
using Infrastructure.Repositories.Entities;
using System.Reflection;
using System.IdentityModel.Tokens;
using Application.Services.Lists;
using Infrastructure.Paging;
using AutoMapper;

namespace Imdb.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Role> _roleRepo;
        private readonly IListService _listService;
        private readonly IMapper _mapper;
        private readonly IRepository<RefreshToken> _refreshTokenRepo;
        public UserService(IUserRepository user, IUnitOfWork unitOfWork, IOptions<JwtSettings> options,IRepository<Role> rolerepo,IListService listService,IMapper mapper,IRepository<RefreshToken> refreshtokenrepo)
        {
            _userRepo = user;
            _jwtSettings = options.Value;
            _unitOfWork = unitOfWork;
            _roleRepo = rolerepo;
            _listService = listService;
            _mapper = mapper;
            _refreshTokenRepo = refreshtokenrepo;
        }

        public async Task<User> UserNameToId(string userName)
        {
            var user = await _userRepo.Query().Where(x => x.UserName == userName).SingleOrDefaultAsync();
            if (user == null) throw new NotFoundException("User");
            return user;
        }
        public async Task<User> EditUser(int userId, RegistrationDTO editUser)
        {
            var user = await _userRepo.GetSingleOrDefaultAsync(userId);
            if (user == null) throw new NotFoundException("User");
            var validationOutput = Validate(editUser);
            if (validationOutput != "True") throw new ValidationException(validationOutput);
            _mapper.Map(editUser, user);
      
            byte[] salt = generateSalt();
            user.Password = EncryptPassword(editUser.Password, salt);
            user.Salt = Convert.ToBase64String(salt);
            await _unitOfWork.SaveChangesAsync();
            return user;
          
        }
        public async Task<User> DeleteUser(int userId,int idToDelete)
        {
            var userToDelete = await _userRepo.QueryRole(userId).Where(x => x.Id == idToDelete).FirstOrDefaultAsync();
            if (userToDelete == null) throw new NotFoundException("User");
            var lists = await _listService.GetListsByUser(null, idToDelete);
            
            
            foreach (var list in lists)
            {
                await _listService.RemoveList(list.Id,userToDelete.Id);
            }
            _userRepo.Remove(userToDelete);
            await _unitOfWork.SaveChangesAsync();
            return userToDelete;
        }
        public async Task<UserTokenDto?> Login(string Username, string Password)
        {
            User? user;            
            user = await _userRepo.Query().Where(x => x.UserName == Username).SingleOrDefaultAsync();            
            if (user == null) throw new AppException("Username was incorrect");

           
            
            if (user.Password != EncryptPassword(Password, Convert.FromBase64String(user.Salt))) throw new Exception("wrong password");
            var jti = Guid.NewGuid().ToString();
            var jwtToken = CreateToken(user,jti);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken();
            refreshTokenEntity.Token = refreshToken;
            refreshTokenEntity.JwtId = jti;
            refreshTokenEntity.CreationDate = DateTime.Now;
            refreshTokenEntity.ExpiryDate = DateTime.Now.AddDays(7);
            refreshTokenEntity.invalidated = false;
            refreshTokenEntity.UserId = user.Id;
            refreshTokenEntity.User = user;
            _refreshTokenRepo.Add(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();
            return new UserTokenDto(jwtToken,refreshToken);
        }
        public async Task<UserTokenDto> RefreshToken(string token,string refreshToken,int userid)
        {
            var refreshTokenEntity = await _refreshTokenRepo.Query().Where(x => x.Token == refreshToken).SingleOrDefaultAsync();
            if (refreshTokenEntity == null) throw new AppException("Wrong refreshToken");
            if (refreshTokenEntity.UserId != userid) throw new ValidationException("User");
            if (refreshTokenEntity.ExpiryDate.CompareTo(DateTime.Now) < 1) throw new AppException("Time expired");
            if (refreshTokenEntity.invalidated == true) throw new AppException("Invalid token");
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            if (refreshTokenEntity.JwtId != jwtSecurityToken.Claims.First(claim => claim.Type == "jti").Value) throw new AppException();
            return new UserTokenDto(CreateToken(await _userRepo.GetSingleOrDefaultAsync(refreshTokenEntity.UserId), refreshTokenEntity.JwtId),refreshToken);

        }
        public async Task RevokeToken(string token)
        {
            var refreshTokenEntity = await _refreshTokenRepo.Query().Where(x => x.Token == token).SingleOrDefaultAsync();
            if (refreshTokenEntity == null) throw new NotFoundException("Token");
            refreshTokenEntity.invalidated = true;
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<User> FindByIdAsync(int id)
        {
            var user = await _userRepo.GetSingleOrDefaultAsync(id);
            if (user == null) throw new NotFoundException("User");
            return user;
        }

        public async Task Register(RegistrationDTO req)
        {
            var CheckUser = await _userRepo.Query().AnyAsync(x => x.UserName == req.UserName);
            if (CheckUser)
            {
                throw new AppException("User already exists");
            }
            var valid = Validate(req);
            if (valid != "True")
            {
                throw new ValidationException(valid);
            }

            byte[] salt = generateSalt();
            string passwordHash = EncryptPassword(req.Password, salt);

            var role = await _roleRepo.Query().Where(x => x.Name == "User").FirstAsync();
            User newUser = new();
            newUser.FirstName = req.FirstName;
            newUser.LastName = req.LastName;
            newUser.UserName = req.UserName;
            newUser.Email = req.Email;
            newUser.Age = int.Parse(req.Age);
            newUser.Password = passwordHash;
            newUser.Salt = Convert.ToBase64String(salt);
            newUser.RoleId= role.Id;

            _userRepo.Add(newUser);
            await _unitOfWork.SaveChangesAsync();
       
        }
        
        public async Task<User> Recover(string token,string NewP,string OldP)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenparams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Key))
            };
            handler.ValidateToken(token, tokenparams, out SecurityToken validtoken);
            if (validtoken == null) throw new ValidationException("Token");
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var stringid = jwtSecurityToken.Claims.First(claim => claim.Type == "nameid").Value;
            if(!int.TryParse(stringid, out int id))throw new NotFoundException("User");
            var user = await _userRepo.Query().Where(x => x.Id == id).FirstAsync();
            if(!(NewP == OldP)) throw new ValidationException("Password");
            var newsalt = generateSalt();
            user.Password = EncryptPassword(NewP,newsalt);
            user.Salt = Convert.ToBase64String(newsalt);
            await _unitOfWork.SaveChangesAsync();
            return user;

        }

        public async Task ForgotPassword(string username,string url)
        {
            User? user = await _userRepo.GetByUserNameAsync(username);
            if (user == null)
            {
                throw new NotFoundException("User");
            }
            MailMessage msg = new();
            SmtpClient smtp = new();
            msg.From = new MailAddress("g.shengelaia2001@gmail.com");
            msg.To.Add(user.Email);
            msg.Subject = "forgot password";
            var jti = Guid.NewGuid().ToString();
            url += "?token=" + CreateToken(user,jti);
            msg.Body = "here is your Link: " + url;
            
            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("anri.alexsandria1@gmail.com", "xaker433");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;           
            smtp.Send(msg);
          
            
        }

        private string Validate(RegistrationDTO req)
        {
            if (!Regex.Match(req.FirstName, @"^[A-Z]?[a-z]+$").Success)
            {
                return "FirstName";
            }
            else if (!Regex.Match(req.LastName, @"^[A-Z]?[a-z]*$").Success)
            {
                return "LastName";
            }
            else if (!Regex.Match(req.UserName, @"^[a-zA-Z]+\d*$").Success)
            {
                return "UserName";
            }
            else if (!Regex.Match(req.Age, @"^\d+$").Success)
            {
                return "Age";
            }
            else if (!Regex.Match(req.Email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$").Success)
            {
                return "Email";
            }
            return "True";
        }

        public async Task ChangePassword(string oldP, string newP, string newPR, int UserId)
        {
            var user = await _userRepo.GetSingleOrDefaultAsync(UserId);
            if (user == null) throw new NotFoundException("User");
            if (user.Password != EncryptPassword(oldP, Convert.FromBase64String(user.Salt))) throw new ValidationException("old Password");
            if (newP != newPR) throw new ValidationException("New Password");
            user.Password = EncryptPassword(newP, Convert.FromBase64String(user.Salt));
            await _unitOfWork.SaveChangesAsync();
        }

        private string EncryptPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                         password: password,
                                                         salt: salt,
                                                         prf: KeyDerivationPrf.HMACSHA256,
                                                         iterationCount: 1000000,
                                                         numBytesRequested: 256 / 8));
        }



        private string CreateToken(User user,string jti)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var userrole = _roleRepo.Query().Where(x => x.Id == user.RoleId).First();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, userrole.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, jti)

                }),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private byte[] generateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            return salt;
        }
    }
}
