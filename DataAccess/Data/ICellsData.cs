namespace DataAccess.Data;

using DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICellsData
{
    Task DeleteCell(int id);
    Task<CellsModel?> GetCell(int id);
    Task<IEnumerable<CellsModel>> GetCells();
    Task InsertCell(CellsModel cellsModel);
    Task UpdateCells(CellsModel cellsModel);
}