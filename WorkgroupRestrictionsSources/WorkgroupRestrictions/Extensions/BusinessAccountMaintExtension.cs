using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.EP;
using PX.TM;

namespace WorkgroupRestrictions
{
	public class BusinessAccountMaintExtension : PXGraphExtension<BusinessAccountMaint>
	{
		[PXViewName(PX.Objects.CR.Messages.BAccount)]
		public PXSelectJoin<BAccount,
			LeftJoin<Contact,
				On<Contact.contactID, Equal<BAccount.defContactID>>>,
			Where2<Match<Current<AccessInfo.userName>>,
				And2<
					Where<BAccount.type, Equal<BAccountType.customerType>,
						Or<BAccount.type, Equal<BAccountType.prospectType>,
						Or<BAccount.type, Equal<BAccountType.combinedType>,
						Or<BAccount.type, Equal<BAccountType.vendorType>>>>>,
					And<Where<
						EntityRestrictionFeature.isEnabled, NotEqual<True>,
						Or<BAccount.createdByID, Equal<Current<AccessInfo.userID>>,
						Or<BAccount.ownerID, Equal<Current<AccessInfo.userID>>,
						Or<BAccount.ownerID, OwnedUser<Current<AccessInfo.userID>>,
						Or<BAccount.noteID, OwnedGroup<BAccount.noteID, Current<AccessInfo.userID>>>>>>>>
				>>>
			BAccount;

		[PXViewName("VisibleTo")]
		public EntityRestrictionAutomation<BAccount.noteID> VisibileTo;
		[PXDBGuid(IsKey = true)]
		[PXDBDefault(typeof(BAccount.noteID))]
		public virtual void EntityWorkgroup_RefNoteID_CacheAttached(PXCache sender) { }

		[PXDimensionSelector("BIZACCT",
			typeof(Search2<BAccount.acctCD,
					LeftJoin<Contact, On<Contact.bAccountID, Equal<BAccount.bAccountID>, And<Contact.contactID, Equal<BAccount.defContactID>>>,
					LeftJoin<Address, On<Address.bAccountID, Equal<BAccount.bAccountID>, And<Address.addressID, Equal<BAccount.defAddressID>>>>>,
				Where2<
					Where<BAccount.type, Equal<BAccountType.customerType>,
						Or<BAccount.type, Equal<BAccountType.prospectType>,
						Or<BAccount.type, Equal<BAccountType.combinedType>,
						Or<BAccount.type, Equal<BAccountType.vendorType>>>>>,
					And<Where<
						EntityRestrictionFeature.isEnabled, NotEqual<True>,
						Or<BAccount.createdByID, Equal<Current<AccessInfo.userID>>,
						Or<BAccount.ownerID, Equal<Current<AccessInfo.userID>>,
						Or<BAccount.ownerID, OwnedUser<Current<AccessInfo.userID>>,
						Or<BAccount.noteID, OwnedGroup<CROpportunity.noteID, Current<AccessInfo.userID>>>>>>>>>>),
			typeof(BAccount.acctCD),
			typeof(BAccount.acctCD), typeof(BAccount.acctName), typeof(BAccount.type), typeof(BAccount.classID), typeof(BAccount.status), typeof(Contact.phone1),
			typeof(Address.city), typeof(Address.countryID), typeof(Contact.eMail))]
		[PXMergeAttributes(Method = MergeMethod.Merge)]
		protected virtual void BAccount_AcctCD_CacheAttached(PXCache cache)
		{
		}
	}
}
