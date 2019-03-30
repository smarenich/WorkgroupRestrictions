﻿using System;
using PX.Data;
using PX.Objects.EP;
using PX.Objects.CR;
using PX.TM;

namespace WorkgroupRestrictions
{
	[System.SerializableAttribute()]
	[PXCacheName("Entity Workgroup")]
	public class EntityWorkgroup : PX.Data.IBqlTable
	{
		#region RestrictionID
		public abstract class restrictionID : PX.Data.IBqlField
		{
		}
		[PXDBIdentity()]
		[PXUIField(DisplayName = "Restriction ID", Visibility = PXUIVisibility.Service)]
		public virtual int? RestrictionID { get; set; }
		#endregion
		#region RefNoteID
		public abstract class refNoteID : PX.Data.IBqlField
		{
		}
		[PXDBGuid(IsKey = true)]
		//[PXRefNote]
		public virtual Guid? RefNoteID { get; set; }
		#endregion
		#region WorkGroupID
		public abstract class workGroupID : PX.Data.IBqlField
		{
		}
		[PXDBInt(IsKey = true)]
		[PXDefault]
		[PXSelector(typeof(Search<EPCompanyTree.workGroupID>),
			SubstituteKey = typeof(EPCompanyTree.description))]

		[PXUIField(DisplayName = "Workgroup")]
		public virtual int? WorkGroupID { get; set; }
		#endregion
	}
}
