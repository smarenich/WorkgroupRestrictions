using PX.Data;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.Common.Attributes;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using PX.TM;
using PX.Objects.CT;

namespace WorkgroupRestrictions
{
	[EntityRestrictionPrimaryGraphAttribute(typeof(SOOrderEntry), typeof(SOOrder.orderNbr))]
	public sealed class SOOrderExt : PXCacheExtension<SOOrder>
	{
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(SO.RefNbrAttribute))]
		[SO.RefNbr(typeof(Search2<SOOrder.orderNbr,
			LeftJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>,
				And<Where<Match<Customer, Current<AccessInfo.userName>>>>>,
			LeftJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrder.orderType>>>>,
			Where<SOOrder.orderType, Equal<Optional<SOOrder.orderType>>,
				And2<Where<SOOrderType.aRDocType, Equal<ARDocType.noUpdate>,
					Or<Customer.bAccountID, IsNotNull>>,
				And<Where<
					EntityRestrictionFeature.isEnabled, NotEqual<True>,
					Or<SOOrder.createdByID, Equal<Current<AccessInfo.userID>>,
					Or<SOOrder.ownerID, Equal<Current<AccessInfo.userID>>,
					Or<SOOrder.ownerID, OwnedUser<Current<AccessInfo.userID>>,
					Or<SOOrder.noteID, OwnedGroup<SOOrder.noteID, Current<AccessInfo.userID>>>>>>>>>>,
			 OrderBy<Desc<SOOrder.orderNbr>>>), Filterable = true)]
		public String OrderNbr { get; set; }
		#endregion
	}
}