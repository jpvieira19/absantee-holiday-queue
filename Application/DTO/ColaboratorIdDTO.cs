namespace Application.DTO;

using Domain.Factory;
using Domain.Model;

public class ColaboratorIdDTO
{
	public long _colabId { get; set; }

    public ColaboratorIdDTO() {
	}

	public ColaboratorIdDTO(long colabId)
	{
		_colabId = colabId;
	}

	static public ColaboratorIdDTO ToDTO(ColaboratorId colaboradorId) {
		long idColab = colaboradorId.colabId;
		ColaboratorIdDTO colaboratorIdDTO = new ColaboratorIdDTO(idColab);

		return colaboratorIdDTO;
	}

	static public IEnumerable<ColaboratorIdDTO> ToDTO(IEnumerable<ColaboratorId> colaboratorIds)
	{
		List<ColaboratorIdDTO> colaboratorsIdDTO = new List<ColaboratorIdDTO>();

		foreach( ColaboratorId colaboratorId in colaboratorIds ) {
			ColaboratorIdDTO holaboratorIdDTO = ToDTO(colaboratorId);

			colaboratorsIdDTO.Add(holaboratorIdDTO);
		}

		return colaboratorsIdDTO;
	}

	static public ColaboratorId ToDomain(ColaboratorIdDTO colaboratorIdDTO) 
	{
		if (colaboratorIdDTO == null) 
		{
			throw new ArgumentException("colaboratorIdDTO must not be null");
		}


		ColaboratorId colaboratorId = new ColaboratorId(colaboratorIdDTO._colabId);

		return colaboratorId;
	}

}