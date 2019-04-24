
-----------------------------------------------------------
--script turning of physics volume
-- Updated 3/15 Darren McKinsey
-----------------------------------------------------------

function onStartup(self)
   self:SetVar("BuildUp", false)
end


function onRebuildNotifyState(self, msg)
   if msg.iState == 2 then
	  local object = self:GetObjectsInGroup{group = "physics_02", ignoreSpawners = true}.objects[1]
      if object then
         object:SetVar("Active", false)
	  end
    end
  end