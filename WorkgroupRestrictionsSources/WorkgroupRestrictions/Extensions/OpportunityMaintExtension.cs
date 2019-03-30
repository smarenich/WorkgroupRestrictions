using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.AR;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.TM;
using PX.Objects.CR.MassProcess;

namespace WorkgroupRestrictions
{
	public class OpportunityMaintExtension : PXGraphExtension<OpportunityMaint>
	{
		[PXCopyPasteHiddenFields(typeof(CROpportunity.details), typeof(CROpportunity.stageID), typeof(CROpportunity.resolution))]
		[PXViewName(PX.Objects.CR.Messages.Opportunity)]
		public PXSelect<CROpportunity,
			Where<
				Where<
					EntityRestrictionFeature.isEnabled, NotEqual<True>,
					Or<CROpportunity.createdByID, Equal<Current<AccessInfo.userID>>,
					Or<CROpportunity.ownerID, Equal<Current<AccessInfo.userID>>,
					Or<CROpportunity.ownerID, OwnedUser<Current<AccessInfo.userID>>,
					Or<CROpportunity.noteID, OwnedGroup<CROpportunity.noteID, Current<AccessInfo.userID>>>>>>>>>
			Opportunity;

		[PXViewName("VisibleTo")]
		public EntityRestrictionAutomation<CROpportunity.noteID, CROpportunity.bAccountID, CROpportunityProducts.inventoryID> VisibileTo;
		[PXDBGuid(IsKey = true)]
		[PXDBDefault(typeof(CROpportunity.noteID))]
		public virtual void EntityWorkgroup_RefNoteID_CacheAttached(PXCache sender) { }
	}
}