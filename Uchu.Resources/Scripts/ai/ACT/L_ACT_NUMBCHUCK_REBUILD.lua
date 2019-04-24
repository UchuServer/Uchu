require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            Generic Rebuild -- Script
--//   - Creates a spawned entity to complete the maelstrom portal mission for Numb Chuck
--///////////////////////////////////////////////////////////////////////////////////////


-- This rebuild resets as soon as it is spawned
function onStartup(self) 
	GAMEOBJ:GetTimer():AddTimerWithCancel( 3.0, "BreakSelf",self )
                RESMGR:LoadObject { objectTemplate =  3076 , x= -263.94   , y= 133.09 , z= 459.125  , owner = self } -- Crystal 1
                RESMGR:LoadObject { objectTemplate =  3076 , x= -257.06   , y= 133.15,  z= 514.45 , owner = self } -- Crystal 2
                RESMGR:LoadObject { objectTemplate =  3000 , x= -292.29   , y= 133.09 , z= 479.82  , owner = self } -- Gas Trigger
end


function onRebuildNotifyState(self, msg)

    -- if we just hit the idle state
	if (msg.iState == 3) then
		-- start a timer that will start the process
		GAMEOBJ:GetTimer():AddTimerWithCancel( 3.0, "BeginScriptedProcess",self )
	end
	if (msg.iState == 2) then
		RESMGR:LoadObject { objectTemplate =  3043 , x= -279.88   , y= 133.10 , z= 489.49 , owner = self } -- Mission Giver
		getID_any(self, "Crystal_1"):Die{ killType = "SILENT" }
		getID_any(self, "Crystal_2"):Die{ killType = "SILENT" }
		getID_any(self, "GasTrigger"):Die{ killType = "SILENT" }
		GAMEOBJ:GetTimer():AddTimerWithCancel( 120, "TimeReset",self )
		
		 
	end	

		
end     

-- Store the parent in the child
function onChildLoaded(self, msg)
	
	    if msg.templateID == 3043 then -- misson
            setID_any(self,msg.childID, "MissionGiver")
            msg.childID:SetParentObj{ bSetToSelf = false }
            msg.childID:FaceTarget{location = {x= -264.037, z=486.601}}
        end
        if   msg.templateID  == 3076 then -- crystal 
            if self:GetVar("Crystal_1") == nil then
                 setID_any(self,msg.childID, "Crystal_1")
            else
                 setID_any(self,msg.childID, "Crystal_2")
            end
        end
        if   msg.templateID  == 3000 then -- gas
                setID_any(self,msg.childID, "GasTrigger")
        end

   
end

onTimerDone = function(self, msg)

	if msg.name == "BreakSelf" then
		self:RebuildReset()
	end
	
    -- Start the scripted break process
    if msg.name == "BeginScriptedProcess" then 

       self:PlayFXEffect{effectType = "rebuild-complete"}	

    end  

    if msg.name == "TimeReset" then
    
                self:RebuildReset()
                getID_any(self, "MissionGiver"):Die{ killType = "SILENT" }
                RESMGR:LoadObject { objectTemplate =  3076 , x= -263.94   , y= 133.09 , z= 459.125  , owner = self } -- Crystal 1
                RESMGR:LoadObject { objectTemplate =  3076 , x= -257.06   , y= 133.15,  z= 514.45 , owner = self } -- Crystal 2
                RESMGR:LoadObject { objectTemplate =  3000 , x= -292.29   , y= 133.09 , z= 479.82  , owner = self } -- Gas Trigger

    end   
	
end


function onMissionDialogueOK(self, msg)
        
            if  self:GetLOT{}.objtemplate ==  3043 then
                GAMEOBJ:GetTimer():CancelAllTimers( self )
                self:RebuildReset()
                getID_any(self, "MissionGiver"):Die{ killType = "SILENT" }
                RESMGR:LoadObject { objectTemplate =  3076 , x= -263.94   , y= 133.09 , z= 459.125  , owner = self } -- Crystal 1
                RESMGR:LoadObject { objectTemplate =  3076 , x= -257.06   , y= 133.15,  z= 514.45 , owner = self } -- Crystal 2
                RESMGR:LoadObject { objectTemplate =  3000 , x= -292.29   , y= 133.09 , z= 479.82  , owner = self } -- Gas Trigger
            end 
end


function setID_any(self,ID, string)
    idString = ID:GetID()
    finalID = "|" .. idString
    self:SetVar(string, finalID)
end

function getID_any(self, string ) 
    targetID = self:GetVar(string)
    return GAMEOBJ:GetObjectByID(targetID)
end 




