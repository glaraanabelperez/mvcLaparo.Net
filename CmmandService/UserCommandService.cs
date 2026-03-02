using AutoMapper;
using CmmandService.Interfaces;
using CmmandService.ModelsCommand;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Models;
using mvc.Laparoscopy.Persistence;
using Repositorys.Interfaces;
using Utils;

namespace CmmandService
{
    public class UserCommandService: IUserCommandService
    {
        private readonly ILogger<UserCommandService> _logger;
        private readonly ApplicationDbContext _dbContext;
        public IGenericRepository commandGeneric;

        public UserCommandService(ApplicationDbContext dbContext, IGenericRepository command, 
            ILogger<UserCommandService> logger)
        {
            _dbContext = dbContext;
            this.commandGeneric = command;
            _logger = logger;
        }

        public async Task<ResultApp<User>> Add(UserCreateCommand command)
        {
            //Buscar si la direccion existe en la bbdd para traer el id, sino crearlo
            ResultApp<User> res = new ResultApp<User>();

            using (IDbContextTransaction transac = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    _dbContext.AddRange(MapToEntity(command));
                    _dbContext.SaveChanges();
                    await transac.CommitAsync();
                    res.Succeeded = true;
                }
                catch (System.Exception ex)
                {
                    await transac.RollbackAsync();
                    string value = ((ex.InnerException != null) ? ex.InnerException!.Message : ex.Message);
                    res.message = ex.Message;
                    _logger.LogWarning(value);
                    throw;
                }
                return res;

            }

        }
        public User MapToEntity(UserCreateCommand command_)
        {
            User entity = new User();

                entity.Email = command_.Email;
                entity.Pass = command_.Pass;
               

            return entity;
        }
      
    }
}

