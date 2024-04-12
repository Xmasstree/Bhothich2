using System;
using System.Collections.Generic;

namespace Bhothich2;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? telegramID { get; set; }

    public string? Answers { get; set;}

    public bool? Alive { get; set; }

    public bool? Bullet { get; set;}

    public int? Points { get; set; }

    public string? TargetId { get; set;}

    public DateTime? TimeOfDeath { get; set; }

    public DateTime? TimeOfShot { get; set; }

    public bool? CorrectOfShot { get; set; }
}
