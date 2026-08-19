namespace LocationService.Domain.Entities;


public class RegionComposition
{
    public string ParentRegionTypeCode { get; set; } = null!;  
    public short ParentRegionCode { get; set; }               
    public string MemberRegionTypeCode { get; set; } = null!;     
    public short MemberRegionCode { get; set; }                    

    public Region ParentRegion { get; set; } = null!;
    public Region MemberRegion { get; set; } = null!;
}
