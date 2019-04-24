---------------------------------------------------------------
--right console to be rebuilt to cause the blue brick to become available
---------------------------------------------------------------



function onStartup(self)
    self:SetVar("IAmBuilt", false)
    self:SetVar("AmActive", false)
end

function onRebuildNotifyState(self, msg)
   if msg.iState == 2 then
      --print("console built")
      self:SetVar("IAmBuilt", true)
      
      local object = self:GetObjectsInGroup{group = "Facility", ignoreSpawners = true}.objects[1]
      if object then
         --print("console built, notifying object")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "ConsoleRightUp", ObjIDSender = self}
      end
   
   elseif msg.iState == 4 then
      self:SetVar("IAmBuilt", false)
      self:SetVar("AmActive", false)
      
      local object = self:GetObjectsInGroup{group = "Facility", ignoreSpawners = true}.objects[1]
      if object then
         --print("console destroyed, notifying object")
         --object:PlayFXEffect{name = "imaginationbase", effectID = 114, effectType = "onrebuild"}
         object:NotifyObject{name = "ConsoleRightDown", ObjIDSender = self}
      end
   end
end

function onUse(self, msg)
    --print("console being used")
    if self:GetVar("AmActive") == true then
		return
	end
	
	if self:GetVar("IAmBuilt") == true then
		self:SetVar("AmActive", true)
		--print("the console has been activated")
		
		local object = self:GetObjectsInGroup{group = "Facility", ignoreSpawners = true}.objects[1]			
		if object then
			--print("console activated")
			object:NotifyObject{name = "ConsoleRightActive", ObjIDSender = self}
		end
	end
	msg.user:TerminateInteraction{type = "fromInteraction", ObjIDTerminator = self}
end