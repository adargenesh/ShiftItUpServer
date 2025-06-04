using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShiftItUpServer.DTO
{
    public class WorkerInShiftDto
    {
        public int Id { get; set; }

        public int ShiftId { get; set; }

        public int WorkerId { get; set; }

        public string? SystemRemarks { get; set; }

        public WorkerDto? Worker { get; set; }

        public WorkerInShiftDto() { }
        public WorkerInShiftDto(Models.WorkerInShift workerInShift)
        {
            Id = workerInShift.Id;
            ShiftId = workerInShift.ShiftId;
            WorkerId = workerInShift.WorkerId;
            SystemRemarks = workerInShift.SystemRemarks;
            if (workerInShift.Worker != null)
            {
                this.Worker = new WorkerDto(workerInShift.Worker);
            }
        }
        public Models.WorkerInShift GetModels()
        {
            return new Models.WorkerInShift
            {
                Id = this.Id,
                ShiftId = this.ShiftId,
                WorkerId = this.WorkerId,
                SystemRemarks = this.SystemRemarks
            };
        }
    }
}
