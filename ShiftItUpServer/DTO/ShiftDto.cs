using ShiftItUpServer.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftItUpServer.DTO
{
    public class ShiftDto
    {
        public int ShiftId { get; set; }

        public DateTime ShiftStart { get; set; }

        public DateTime ShiftEnd { get; set; }

        public decimal SalesGoal { get; set; }

        public decimal? SalesActual { get; set; }

        public int NumEmployees { get; set; }

        public List<WorkerInShiftDto> WorkerInShifts { get; set; } = new List<WorkerInShiftDto>();

        public ShiftDto() { }
        public ShiftDto(Models.Shift shift)
        {
            ShiftId = shift.ShiftId;
            ShiftStart = shift.ShiftStart;
            ShiftEnd = shift.ShiftEnd;
            SalesGoal = shift.SalesGoal;
            SalesActual = shift.SalesActual;
            NumEmployees = shift.NumEmployees;
            if (shift.WorkerInShifts != null)
            {
                WorkerInShifts = new List<WorkerInShiftDto>();
                foreach (var workerInShift in shift.WorkerInShifts)
                {
                    WorkerInShifts.Add(new WorkerInShiftDto(workerInShift));
                }
            }
        }

        public Models.Shift GetModels()
        {
            return new Models.Shift
            {
                ShiftId = this.ShiftId,
                ShiftStart = this.ShiftStart,
                ShiftEnd = this.ShiftEnd,
                SalesGoal = this.SalesGoal,
                SalesActual = this.SalesActual,
                NumEmployees = this.NumEmployees
            };
        }
    }
}
