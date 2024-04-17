using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Repository;
using Domain.IRepository;

namespace Application.Services
{
    public class ColaboratorIdService
    {
        private readonly AbsanteeContext _context;

        private readonly IColaboratorsIdRepository _colaboratorsIdRepository;
        
        public ColaboratorIdService(IColaboratorsIdRepository colaboratorsIdRepository) {
            _colaboratorsIdRepository = colaboratorsIdRepository;
        }

        public async Task<long> Add(long colabId)
        {

            return await _colaboratorsIdRepository.Add(colabId);
        }
    }
}