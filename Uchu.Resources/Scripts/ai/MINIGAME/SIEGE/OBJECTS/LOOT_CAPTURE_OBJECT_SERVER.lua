function onUnEquipItem(self, msg)
  local player = self:GetItemOwner{}.ownerID
    if (msg) then
         GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "reSetAnimationSet" , paramObj = player }
    end 
  

end

function onNotifyObject(self,msg)
    local player = self:GetItemOwner{}.ownerID
    GAMEOBJ:GetZoneControlID():NotifyClientZoneObject{name = "SetAnimationSet" , paramStr = "carry" , paramObj = player }

end

function onStartup(self)

	print( tostring(self:GetVar("taken_name")) )

end