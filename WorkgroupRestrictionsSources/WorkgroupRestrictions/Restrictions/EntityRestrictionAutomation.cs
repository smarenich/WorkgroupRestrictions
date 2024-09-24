using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.SQLTree;
using PX.Objects;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.SM;
using PX.TM;
using PX.DbServices.QueryObjectModel;

namespace WorkgroupRestrictions
{
	public class OwnedGroup<FieldNote, OperandUser> : IBqlComparison
		where FieldNote : IBqlField
		where OperandUser : IBqlOperand, new()
	{
		private IBqlCreator _operandUser;
		public void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value)
		{
			result = null;
			value = null;
		}

		public virtual bool AppendExpression(ref SQLExpression exp, PXGraph graph, BqlCommandInfo info, BqlCommand.Selection selection)
		{
			bool status = true;

			if (info.Fields is BqlCommand.EqualityList list) list.NonStrict = true;

			SQLExpression opUser = null;
			if (!typeof(IBqlCreator).IsAssignableFrom(typeof(OperandUser)))
			{
				if (info.BuildExpression) opUser = BqlCommand.GetSingleExpression(typeof(OperandUser), graph, info.Tables, selection, BqlCommand.FieldPlace.Condition);
				info.Fields?.Add(typeof(OperandUser));
			}
			else
			{
				if (_operandUser == null) _operandUser = _operandUser.createOperand<OperandUser>();
				status &= _operandUser.AppendExpression(ref opUser, graph, info, selection);
			}

			Query qin = new Query();
			qin[typeof(EPCompanyTreeH.workGroupID)].From(typeof(EPCompanyTreeH))
				.Join(typeof(EPCompanyTreeMember))
				.On(SQLExpression.EQ(typeof(EPCompanyTreeH.parentWGID), typeof(EPCompanyTreeMember.workGroupID))
					//.And(Column.SQLColumn(typeof(EPCompanyTreeH.parentWGID)).NotEqual(Column.SQLColumn(typeof(EPCompanyTreeH.workGroupID)))
					.And(Column.SQLColumn(typeof(EPCompanyTreeMember.active)).EQ(1))
					.And(Column.SQLColumn(typeof(EPCompanyTreeMember.userID)).EQ(opUser)))
				//)
				.Where(new SQLConst(1).EQ(1));

			Query qout = new Query();
			//Append Tail removes main object, so we fieldNote will not be mapped. Skipping conditions for AppendTail
			if (info.Tables == null || info.Tables.Count <= 0 || info.Tables.Contains(BqlCommand.GetItemType<FieldNote>()))
			{
				qout[typeof(EntityWorkgroup.refNoteID)].From(typeof(EntityWorkgroup))
					.Where(Column.SQLColumn(typeof(EntityWorkgroup.workGroupID)).In(qin)
						.And(SQLExpression.EQ(typeof(EntityWorkgroup.refNoteID), typeof(FieldNote))));
			}
			else
			{
				qout[typeof(EntityWorkgroup.refNoteID)].From(typeof(EntityWorkgroup))
					.Where(Column.SQLColumn(typeof(EntityWorkgroup.workGroupID)).In(qin));
			}

			qout.Limit(-1); // prevent limiting of IN subqueries
			exp = exp.In(qout);

			return status;
		}
	}

	public class EntityRestrictionAutomation<SourceNote>
		: EntityRestrictionAutomation<SourceNote, SourceNote, SourceNote>
		where SourceNote : class, IBqlField
	{
		public EntityRestrictionAutomation(PXGraph graph, Delegate @delegate)
			: base(graph, @delegate)
		{
		}
		public EntityRestrictionAutomation(PXGraph graph)
			: base(graph)
		{
		}
	}

	public class EntityRestrictionAutomation<SourceNote, BAccountField, InventoryField>
		: PXSelect<EntityWorkgroup,
			Where<EntityWorkgroup.refNoteID, Equal<Current<SourceNote>>>>
		where SourceNote : class, IBqlField
		where BAccountField : class, IBqlField
		where InventoryField : class, IBqlField
	{
		public EntityRestrictionAutomation(PXGraph graph, Delegate @delegate)
			: base(graph, @delegate)
		{
			Initialize(graph);
		}
		public EntityRestrictionAutomation(PXGraph graph)
			: base(graph)
		{
			Initialize(graph);
		}
		private void Initialize(PXGraph graph)
		{
			graph.Initialized += InitLastEvents;
		}
		private void InitLastEvents(PXGraph graph)
		{
			if (typeof(SourceNote) != typeof(BAccountField))
				graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(typeof(BAccountField)), typeof(BAccountField).Name, BAcclountID_FieldUpdated);

			if (typeof(SourceNote) != typeof(InventoryField))
				graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(typeof(InventoryField)), typeof(InventoryField).Name, InventoryID_FieldUpdated);
		}

		protected virtual void BAcclountID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Int32? OldID = (Int32?)e.OldValue;
			if (OldID != null)
			{
				BAccountR obj = (BAccountR)PXSelectorAttribute.Select<BAccountField>(cache, e.Row, OldID);
				Int32? workgroup = obj?.WorkgroupID;
				if (workgroup != null)
				{
					foreach (EntityWorkgroup wg in this.Search<EntityWorkgroup.workGroupID>(workgroup))
					{
						this.Delete(wg);
					}
				}
			}

			Int32? NewID = (Int32?)cache.GetValue<BAccountField>(e.Row);
			if (NewID != null)
			{
				BAccountR obj = (BAccountR)PXSelectorAttribute.Select<BAccountField>(cache, e.Row, NewID);
				Int32? workgroup = obj?.WorkgroupID;
				if (workgroup != null)
				{
					EntityWorkgroup wg = (EntityWorkgroup)this.Cache.CreateInstance();
					wg.WorkGroupID = workgroup;
					wg = this.Insert(wg);
				}
			}
		}
		protected virtual void InventoryID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Int32? NewID = (Int32?)cache.GetValue<InventoryField>(e.Row);
			if (NewID != null)
			{
				InventoryItem obj = (InventoryItem)PXSelectorAttribute.Select<InventoryField>(cache, e.Row, NewID);
				Int32? workgroup = obj?.ProductWorkgroupID;
				if (workgroup != null)
				{
					EntityWorkgroup wg = (EntityWorkgroup)this.Cache.CreateInstance();
					wg.WorkGroupID = workgroup;
					wg = this.Insert(wg);
				}
			}
		}
	}

	public static class EntityRestrictionFeature
	{
		public class isEnabled : PX.Data.BQL.BqlBool.Constant<isEnabled>
		{
			public isEnabled()
				: base(IsEnabled())
			{
			}
		}

		public static bool IsEnabled()
		{
			return PXDatabase.GetSlot<EntityRestrictionDefinition>(typeof(EntityRestrictionDefinition).Name, typeof(PreferencesSecurity)).Enabled;
		}

		public class EntityRestrictionDefinition : IPrefetchable
		{
			public bool Enabled { get; set; }
			public void Prefetch()
			{
				using (PXDataRecord rec = PXDatabase.SelectSingle<PreferencesSecurity>(new PXDataField<PreferencesSecurityExt.usrApplyWorkgroupRestrictions>()))
				{
					if (rec != null) Enabled = rec.GetBoolean(0) ?? false;
				}
			}
		}
	}

	public class EntityRestrictionPrimaryGraphAttribute : PXPrimaryGraphAttribute
	{
		protected Type PrimarySelector = null;

		public EntityRestrictionPrimaryGraphAttribute(Type type, Type primarySelector)
			: base(type)
		{
			PrimarySelector = primarySelector;
		}

		public override Type GetGraphType(PXCache cache, ref object row, bool checkRights, Type preferedType)
		{
			Type ret = base.GetGraphType(cache, ref row, checkRights, preferedType);

			if(cache.GetStatus(row) != PXEntryStatus.Inserted)
				CheckAccess(row, ret, PrimarySelector);

			return ret;
		}

		public static void CheckAccess(object row, Type ret, Type primaryselector)
		{
			if (ret != null && row != null && BqlCommand.GetItemType(primaryselector).IsAssignableFrom(row.GetType()))
			{
				//TOCHECK Possible Perfornance Issue - this will select record for each search result.
				PXGraph graph = PXGraph.CreateInstance(ret);
				Object validation = PXSelectorAttribute.Select(graph.GetPrimaryCache(), row, primaryselector.Name);
				if (validation == null)
					throw new PXNotEnoughRightsException(PXCacheRights.Select);
			}
		}
	}
	public class EntityRestrictionCRCacheIndependentPrimaryGraphAttribute : CRCacheIndependentPrimaryGraphListAttribute
	{
		protected Type PrimarySelector = null;

		public EntityRestrictionCRCacheIndependentPrimaryGraphAttribute(Type[] graphTypes, Type[] condition, Type primarySelector)
			: base(graphTypes, condition)
		{
			PrimarySelector = primarySelector;
		}

		public override Type GetGraphType(PXCache cache, ref object row, bool checkRights, Type preferedType)
		{
			Type ret = base.GetGraphType(cache, ref row, checkRights, preferedType);

      			if(cache.GetStatus(row) != PXEntryStatus.Inserted)
				EntityRestrictionPrimaryGraphAttribute.CheckAccess(row, ret, PrimarySelector);

			return ret;
		}
	}
}
