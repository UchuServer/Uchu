--------------------------------------------------------------
-- Client side script for the NJ spinners
--
-- created mrb... 11/13/10 
--------------------------------------------------------------
--function onStartup(self)
--	self:SetProximityRadius{iconID = 97, radius = 80, name = "Icon_Display_Distance"}
--end

-- animations on use for playerlocal preloadAnims = {	"spinjitzu-staff-windup", "spinjitzu-staff-loop", "spinjitzu-staff-end", 
						"spinjitzu-staff-windup-down", "spinjitzu-staff-loop-down", "spinjitzu-staff-end-down",
						"up", "down", "idle", "idle-up"}

----------------------------------------------
-- sent when the local player interacts with the
-- object before ClientUse
----------------------------------------------
function onCheckUseRequirements(self, msg)
    if self:GetNetworkVar('bIsInUse') then 
        msg.bCanUse = false
        
        return msg
    end
    
    local preConVar = self:GetVar("CheckPrecondition")
    
    if preConVar and preConVar ~= "" then
        local check = msg.objIDUser:CheckListOfPreconditionsFromLua{PreconditionsToCheck = preConVar, requestingID = self}
        
        if not check.bPass then 
			if msg.isFromUI then
				msg.HasReasonFromScript = true
				msg.Script_IconID = check.IconID
				msg.Script_Reason = check.FailedReason
				msg.Script_Failed_Requirement = true
			end
			
            msg.bCanUse = false
            
            return msg
        end
    end
    
    local preconditionID = self:GetNetworkVar("element_precondition_ID")
    local check = {}
    
    if preconditionID then
		check = msg.objIDUser:CheckPrecondition{PreconditionID = preconditionID}
	end		
		
	if not check.bPass then 
		if msg.isFromUI then
			msg.HasReasonFromScript = true
			msg.Script_IconID = check.IconID
			msg.Script_Reason = check.FailedReason
			msg.Script_Failed_Requirement = true
		end
		
		msg.bCanUse = false
	end
	
	return msg
end

function onClientUse(self, msg)
	freezePlayer(self, true)
end
function onScopeChanged(self, msg)
	if not msg.bEnteredScope then return end
	
	local player = GAMEOBJ:GetControlledID()
	
	for k,anim in ipairs(preloadAnims) do
		player:PreloadAnimation{animationID = anim}
	end
end

function onScriptNetworkVarUpdate(self, msg)
    local player = GAMEOBJ:GetControlledID()	
	for varName,varValue in pairs(msg.tableOfVars) do
		-- check to see if we have the correct message and deal with it
		if varName == "bIsInUse" then 
			self:RequestPickTypeUpdate()
			if not varValue then				player:TerminateInteraction{type = 'fromInteraction', ObjIDTerminator = self}			end
		elseif varName == "bInteractive" then
			self:RequestPickTypeUpdate()
		elseif string.starts(varName, "bFreezePlayer") then 
			if string.sub(varName,string.len("bFreezePlayer_")+1) == player:GetID() then
				freezePlayer(self, varValue)
			end
		elseif varName == "current_anim" then
			-- play the up animation
			self:PlayAnimation{ animationID = varValue, fPriority = 4.0}				end
	end
end
----------------------------------------------
-- sent when the object checks it's pick type
----------------------------------------------
function onGetPriorityPickListType(self, msg)  
    if self:GetVar("static") then 
		msg.ePickType = -1
		
		return msg
	end
	
	local myPriority = 0.8  
    if ( myPriority > msg.fCurrentPickTypePriority ) then    
        msg.fCurrentPickTypePriority = myPriority  
        if self:GetNetworkVar('bIsInUse') or not self:GetNetworkVar('bInteractive') then
            msg.ePickType = -1
        else
            msg.ePickType = 14    -- Interactive pick type     
        end
    end    
    return msg
end 

function freezePlayer(self, bFreeze)    
    local playerID = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())  
    local eChangeType = "POP"
    
    if bFreeze then
        if playerID:IsDead().bDead then
            --print('frozen')
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "Try_Freeze_Again", self )
            
            return
        end

        eChangeType = "PUSH"
    else	
        if playerID:IsDead().bDead then
            --print('frozen')
            GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1 , "Try_UnFreeze_Again", self )
            
            return
        end
	end
    
    playerID:SetStunned{ StateChangeType =  eChangeType,
                                            bCantMove = true,
                                            bCantAttack = true,
                                            bCantInteract = true,
											bCantUseItem = true}
end

function onTimerDone(self, msg)
    if msg.name == "Try_Freeze_Again" then    
        freezePlayer(self, true)
    elseif msg.name == "Try_UnFreeze_Again" then    
        freezePlayer(self, false)
    end
end 

function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end 