using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Data.EP;
using PX.Objects.CR.MassProcess;
using PX.TM;

namespace WorkgroupRestrictions
{
	[EntityRestrictionPrimaryGraphAttribute(typeof(OpportunityMaint), typeof(CROpportunity.opportunityID))]
	public sealed class CROpportunityExtension : PXCacheExtension<CROpportunity>
	{
		#region OpportunityID
		public abstract class opportunityID : PX.Data.IBqlField { }

		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(PXSelectorAttribute))]
		[PXSelector(typeof(Search2<CROpportunity.opportunityID,
			LeftJoin<BAccount, On<BAccount.bAccountID, Equal<CROpportunity.bAccountID>>,
			LeftJoin<Contact, On<Contact.contactID, Equal<CROpportunity.contactID>>>>,
			Where<
				Where<
					EntityRestrictionFeature.isEnabled, NotEqual<True>,
					Or<CROpportunity.createdByID, Equal<Current<AccessInfo.userID>>,
					Or<CROpportunity.ownerID, Equal<Current<AccessInfo.userID>>,
					Or<CROpportunity.ownerID, OwnedUser<Current<AccessInfo.userID>>,
					Or<CROpportunity.ownerID, OwnedGroup<CROpportunity.noteID, Current<AccessInfo.userID>>>>>>>>,
			OrderBy<Desc<CROpportunity.opportunityID>>>),
			new[]
			{
				typeof(CROpportunity.opportunityID),
				typeof(CROpportunity.subject),
				typeof(CROpportunity.status),
				typeof(CROpportunity.curyAmount),
				typeof(CROpportunity.curyID),
				typeof(CROpportunity.closeDate),
				typeof(CROpportunity.stageID),
				typeof(CROpportunity.classID),
				typeof(BAccount.acctName),
				typeof(Contact.displayName)
			},
			Filterable = true)]
		public String OpportunityID { get; set; }
		#endregion
	}
}