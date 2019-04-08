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
using PX.Objects;

namespace WorkgroupRestrictions
{
	[EntityRestrictionCRCacheIndependentPrimaryGraphAttribute(new Type[]{
		typeof(PX.Objects.CR.BusinessAccountMaint),
		typeof(PX.Objects.EP.EmployeeMaint),
		typeof(PX.Objects.AP.VendorMaint),
		typeof(PX.Objects.AP.VendorMaint),
		typeof(PX.Objects.AR.CustomerMaint),
		typeof(PX.Objects.AR.CustomerMaint),
		typeof(PX.Objects.AP.VendorMaint),
		typeof(PX.Objects.AR.CustomerMaint),
		typeof(PX.Objects.CR.BusinessAccountMaint)},
		new Type[]{
			typeof(Select<PX.Objects.CR.BAccount, Where<PX.Objects.CR.BAccount.bAccountID, Equal<Current<BAccount.bAccountID>>,
					And<Current<BAccount.viewInCrm>, Equal<True>>>>),
			typeof(Select<PX.Objects.EP.EPEmployee, Where<PX.Objects.EP.EPEmployee.bAccountID, Equal<Current<BAccount.bAccountID>>>>),
			typeof(Select<PX.Objects.AP.VendorR, Where<PX.Objects.AP.VendorR.bAccountID, Equal<Current<BAccount.bAccountID>>>>),
			typeof(Select<PX.Objects.AP.Vendor, Where<PX.Objects.AP.Vendor.bAccountID, Equal<Current<BAccountR.bAccountID>>>>),
			typeof(Select<PX.Objects.AR.Customer, Where<PX.Objects.AR.Customer.bAccountID, Equal<Current<BAccount.bAccountID>>>>),
			typeof(Select<PX.Objects.AR.Customer, Where<PX.Objects.AR.Customer.bAccountID, Equal<Current<BAccountR.bAccountID>>>>),
			typeof(Where<PX.Objects.CR.BAccountR.bAccountID, Less<Zero>,
					And<BAccountR.type, Equal<BAccountType.vendorType>>>),
			typeof(Where<PX.Objects.CR.BAccountR.bAccountID, Less<Zero>,
					And<BAccountR.type, Equal<BAccountType.customerType>>>),
			typeof(Select<PX.Objects.CR.BAccount,
				Where<PX.Objects.CR.BAccount.bAccountID, Equal<Current<BAccount.bAccountID>>,
					Or<Current<BAccount.bAccountID>, Less<Zero>>>>)
		}, typeof(BAccount.acctCD))]
	public sealed class BAccountExt : PXCacheExtension<BAccount>
	{
	}
}