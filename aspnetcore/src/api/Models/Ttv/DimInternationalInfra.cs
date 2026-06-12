using System;
using System.Collections.Generic;

namespace api.Models.Ttv;

public partial class DimInternationalInfra
{
    public int Id { get; set; }

    public string NameEn { get; set; }

    public string UnlinkedIdentifier { get; set; }

    public string Weblink { get; set; }

    public string SourceId { get; set; }

    public string SourceDescription { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? Modified { get; set; }

    public virtual ICollection<FactRelation> FactRelationFromInternationalInfras { get; set; } = new List<FactRelation>();

    public virtual ICollection<FactRelation> FactRelationToInternationalInfras { get; set; } = new List<FactRelation>();
}
