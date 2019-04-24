function onFactionTriggerItemEquipped (self)
    self:AddStatTrigger{ Name="Low Health", Stat="HEALTH", Operator="LESS_EQUAL", Value=1 }

end

function onStatEventTriggered(self, msg)
    -- the Parent of the equipment (the player)
    local parent = msg.Parent
    -- the sender of this message (the equipment)
   -- local sender = msg.Sender
    -- the name of the trigger -- this is can be used to identify the trigger to perform the desired action(s)
    --local name = msg.Name
    -- the stat which changed
   -- local stat = msg.Stat
    -- the value of the stat
    local statValue = msg.StatValue
    -- the maximum value of the stat
  --  local totalValue = msg.TotalValue
    if statValue ~= msg.OldValue then
      parent:CastSkill{skillID = 504}
    end
end