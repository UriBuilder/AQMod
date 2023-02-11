﻿using System;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Aequus.Common.ItemDrops
{
    public class FuncNPCInstanceConditional : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public Func<NPC, bool> condition;
        public string textKey;
        public readonly string Key;

        public FuncNPCInstanceConditional(Func<NPC, bool> condition, string internalKey, string textKey)
        {
            this.condition = condition;
            Key = internalKey;
            this.textKey = textKey;
        }

        public virtual bool CanDrop(DropAttemptInfo info)
        {
            if (info.npc == null)
                return true;
            return condition(info.npc);
        }

        public virtual bool CanShowItemDropInUI()
        {
            return false;
        }

        public virtual string GetConditionDescription()
        {
            return textKey == null ? null : Language.GetTextValue(textKey);
        }
    }
}