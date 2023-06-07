using optimizely_cms12_azure_ad.Models.ViewModels;

namespace optimizely_cms12_azure_ad.Business;

/// <summary>
/// Defines a method which may be invoked by PageContextActionFilter allowing controllers
/// to modify common layout properties of the view model.
/// </summary>
internal interface IModifyLayout
{
    void ModifyLayout(LayoutModel layoutModel);
}
