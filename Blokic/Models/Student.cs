using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blokic.Models
{
    public partial class Student
    {
        public Student()
        {
            Enrolment = new HashSet<Enrolment>();
        }

        public int Id { get; set; }
        public string Jmbag { get; set; }
        public string IndexNmb { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public virtual ICollection<Enrolment> Enrolment { get; set; }

        public IDictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(Jmbag) || Jmbag.Length != 10 || !Jmbag.All(_ => char.IsDigit(_)))
            {
                errors.Add(nameof(Jmbag),"Invalid JMBAG.");
            }

            if (string.IsNullOrWhiteSpace(Firstname))
            {
                errors.Add(nameof(Firstname), "Firstname is required.");
            }

            if (string.IsNullOrWhiteSpace(Lastname))
            {
                errors.Add(nameof(Lastname), "Lastname is required.");
            }

            return errors;
        }
    }
}
