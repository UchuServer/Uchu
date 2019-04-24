------------------------------------------------
--switch to fire EMP
------------------------------------------------
require('o_mis')
function onStartup(self)
   self:AddObjectToGroup{group = "bossButtons"}

end

function onObjectActivated(self,msg)
	--------------------------------------
	--   Store Activity Object if nil   --
	--------------------------------------
	
	--SendNetWorkVar( self , "Green"  , "", "", "", "", "", "" )
	
	if not self:GetVar("ActivityObj") then
	
	 	local ActivityObj = self:GetObjectsInGroup{ group = "ActivityObj" ,ignoreSpawners = true }.objects
	 	if ActivityObj[1] then
            ActivityObj[1]:NotifyObject{name = "storeIntroTrigger", ObjIDSender = self }	
            storeObjectByName(self, "Activity", ActivityObj[1])
	 	end
	end

    if msg.activatorID:IsCharacter().isChar then
    	
    	local ActivityObj = getObjectByName(self, "Activity")
    	
    	ActivityObj:NotifyObject{name = "butttonDown"}
    	
    end

end


function onObjectDeactivated(self,msg)
	--SendNetWorkVar( self , "Red"  , "", "", "", "", "", "" )
    if msg.deactivatorID:IsCharacter().isChar then
    
    	local ActivityObj = getObjectByName(self, "Activity")
    	
    	ActivityObj:NotifyObject{name = "butttonUp"}
    
    end

end