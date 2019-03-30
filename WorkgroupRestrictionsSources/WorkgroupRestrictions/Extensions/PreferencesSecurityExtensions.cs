using PX.Data;
using PX.Objects;
using PX.SM;
using System;

namespace WorkgroupRestrictions
{
	public sealed class PreferencesSecurityExt : PXCacheExtension<PX.SM.PreferencesSecurity>
	{
		#region UsrApplyWorkgroupRestrictions
		[PXDBBool]
		[PXUIField(DisplayName = "Apply Workgroup Restrictions")]
		public bool? UsrApplyWorkgroupRestrictions { get; set; }
		public abstract class usrApplyWorkgroupRestrictions : IBqlField { }
		#endregion
	}
}