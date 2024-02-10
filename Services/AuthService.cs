using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace TestApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWT>jwt)
        {
            _userManager = userManager;
            _jwt =jwt.Value;
        }
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email)is not null)
                return new AuthModel { Message = "Email is already registered !!"};

            if (await _userManager.FindByEmailAsync(model.UserName) is not null)
                return new AuthModel { Message = "UserName is already registered !!" };

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FristName = model.FristName,
                LastName = model.LastName
               
            };

            var result = await _userManager.CreateAsync(user,model.Password);
            if (!result.Succeeded) 
            {
                var errors =string.Empty;
                foreach (var error in result.Errors) 
                {
                    errors += error.Description;


                }
                return new AuthModel { Message = "Errors!!" };
            }
            await _userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken = await CreateJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> {"user"},
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,

            };

        }
        public async Task<AuthModel> LoginAsync(LoginModel model)
        {
            var authModel = new AuthModel ();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || await _userManager.CheckPasswordAsync(user,model.Password) ) 
            {
                authModel.Message = "Email or Password is incorrect" ;
                return authModel;

            }
            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.ExpiresOn= jwtSecurityToken.ValidTo;
            authModel.Roles=rolesList.ToList();

            return authModel;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims =await _userManager.GetClaimsAsync(user);
            var roles=await _userManager.GetRolesAsync(user);
            var roleClaims=new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));



            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid",user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecuritKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecuritKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(

                issuer: _jwt.Issuer,
                audience:_jwt.Audience,
                claims : claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);



            return jwtSecurityToken;





        }
        
    }
    
}
