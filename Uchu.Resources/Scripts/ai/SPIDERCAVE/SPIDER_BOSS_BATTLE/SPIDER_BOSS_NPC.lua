require('State')
require('o_StateCreate')
require('o_mis')
require('ai/SPIDERCAVE/SPIDER_BOSS_BATTLE/SPIDER_Main')
CONSTANTS = {}
CONSTANTS["NO_OBJECT"] = "0"


function onStartup(self) 
Set = {}
	self:SetVar("Shielded" , true )
  
    -- use native code AI?
    Set['SuspendLuaAI']	   = true	      -- a state suspending scripted AI
    Set['SuspendLuaMovementAI']	   = false      -- a state suspending scripted movement AI
    

--[[
///////////////////////////////////////////////////////////////////////////
         ____       _      ____    ___   _   _   ____  
        |  _ \     / \    |  _ \  |_ _| | | | | / ___| 
        | |_) |   / _ \   | | | |  | |  | | | | \___ \ 
        |  _ <   / ___ \  | |_| |  | |  | |_| |  ___) |
        |_| \_\ /_/   \_\ |____/  |___|  \___/  |____/         
--]]

    Set['aggroRadius']      = 30     -- Aggro Radius
    Set['conductRadius']    = 15     -- Conduct Radius
    Set['tetherRadius']     = 300     -- Tether  Radius
    Set['tetherSpeed']      = 8      -- Tether Speed
    Set['wanderRadius']     = 8      -- Wander Radius
    --- FOV Radius -- 
    -- Aggro
    Set['UseAggroFOV']      = false
    Set['aggroFOV']         = 180 
    -- Conduct
    Set['UseConductFOV']    = false
    Set['conductFOV']       = 180 
--[[
///////////////////////////////////////////////////////////////////////////
         __  __    ___   __     __  _____   __  __   _____   _   _   _____ 
        |  \/  |  / _ \  \ \   / / | ____| |  \/  | | ____| | \ | | |_   _|
        | |\/| | | | | |  \ \ / /  |  _|   | |\/| | |  _|   |  \| |   | |  
        | |  | | | |_| |   \ V /   | |___  | |  | | | |___  | |\  |   | |  
        |_|  |_|  \___/     \_/    |_____| |_|  |_| |_____| |_| \_|   |_|  
--]]

    --**********************************************************************
    Set['MovementType']     = "Guard" --["Guard"],["Wander"]
    --**********************************************************************
    -- Attach Way Point Set to NPC -- " this is for NPC's that are not HF placed " 
    Set['WayPointSet']      =  nil
    -- Wander Settings ---------------------------------------------w------------
    Set['WanderChance']      = 0          -- Main Weight
    Set['WanderDelayMin']    = 5            -- Min Wander Delay
    Set['WanderDelayMax']    = 5            -- Max Wander Delay
    Set['WanderSpeed']       = 0.5          -- Move speed 
    Set['wanderRadius']		 = 50			-- Wander Radius
    -- effect 1
    Set['WanderEmote']       = false        -- Enable bool
    Set['WEmote_1']          = 30           -- Weight 
    Set['WEmoteType_1']      = "salute"     -- Animation Type
	

------ Set your Custom ProximityRadius            -----------------------------

 --self:SetProximityRadius { radius = 10 , name = "CustomRadius" }
	
------ Do not change ----------------------------------------------------------
    self:SetVar("Set",Set)
    loadOnce(self) 
    getVarables(self)
    CreateStates(self)
    oStart(self)
    
    self:SetVar("Phase", 1 )
 
--------------------------------------------------------------------------------


 		Phase = {}
	
	 	local ActivityObj = self:GetObjectsInGroup{ group = "ActivityObj" ,ignoreSpawners = true }.objects
	

	 	Phase[1] = ActivityObj[1]:GetVar("con.Phase_1_health")
	 	Phase[2] = ActivityObj[1]:GetVar("con.Phase_2_health")	
	 	Phase[3] = ActivityObj[1]:GetVar("con.Phase_3_health") 	
	 	Phase[4] = ActivityObj[1]:GetVar("con.Phase_Death") 	

	
	
end
function onTemplateNotifyObject(self,msg)

	if msg.name == "Stunned" then
		

		
		self:SetVar("BossStage", 1)
		self:SetVar("Shielded" , false )
		self:SetArmor{armor = 0 }

		SendNetWorkVar( self , "removeArmor"  , "", "", "", "", "", "" )
		
		  local meItem = self:GetInventoryItemInSlot().itemID
		  self:UnEquipInventory{ itemtounequip = meItem}				


	
		
		
		self:ChangeIdleFlags{on = 13}
		local eChangeType = "PUSH"
		self:SetStunned{ StateChangeType = eChangeType,
		bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true }
	elseif msg.name == "SetShield" then
	
			
	
			
		  
		 	self:SetVar("Shielded" , true )
			local eChangeType = "POP"
			self:SetStunned{ StateChangeType = eChangeType,
			bCantMove = true, bCantTurn = true, bCantAttack = true, bCantEquip = true, bCantInteract = true }	
			
			local meItem = self:GetInventoryItemInSlot().itemID
		  	self:EquipInventory{ itemtoequip = meItem}
		  	self:ChangeIdleFlags{off = 13}
	end


end


function onTemplateHit (self, msg ) 
	
	--self:PlayFXEffect{ effectID = 975, effectType = "onhit"}
	
	SendNetWorkVar( self , "SetHealth"  , "", "", "", "", "", "" )
    local health = self:GetHealth{}.health
	local armor = self:GetArmor{}.armor
	
	
	if  self:GetVar("Shielded") and armor < 35 then
	
		self:SetArmor{armor = 40 }
		
		local meItem = self:GetInventoryItemInSlot().itemID
		if not meItem:IsItemEquipped{}.bIsEquipped then
				self:EquipInventory{ itemtoequip = meItem}
		end
		
	else
	
	
	
		if health <= Phase[1] and not self:GetVar("Shielded") and self:GetVar("Phase") == 1 then 

				local ActivityObj = self:GetParentObj().objIDParent

				ActivityObj:NotifyObject{name = "PhaseOneComplete" }
				ActivityObj:ActivityTimerStopAllTimers()



		elseif health <= Phase[2] and not self:GetVar("Shielded") and self:GetVar("Phase") == 2 then 

				local ActivityObj = self:GetParentObj().objIDParent
				ActivityObj:NotifyObject{name = "PhaseTwoComplete" }
				ActivityObj:ActivityTimerStopAllTimers()



		elseif health <= Phase[3] and not self:GetVar("Shielded") and self:GetVar("Phase") == 3 then 

				local ActivityObj = self:GetParentObj().objIDParent
				ActivityObj:ActivityTimerStopAllTimers()

		end
		
	end
		
end 
function onTemplateDie (self, msg ) 

        local ActivityObj = self:GetParentObj().objIDParent
    
		ActivityObj:ActivityTimerStop{name = "Phase_3"}
        ActivityObj:NotifyObject{name = "BossDead" }
        
        
		local Markpos = self:GetPosition().pos 
		local Markrot = self:GetRotation()		
		local posString = self:CreatePositionString{ x = Markpos.x, y = Markpos.y, z = Markpos.z }.string
		local config = { {"rebuild_activators", posString }, {"respawn", 10000 }, {"rebuild_reset_time", -1}, {"no_timed_spawn", true}, {"currentTime", 0}  }
		

		RESMGR:LoadObject { objectTemplate = 9501  , x = Markpos.x ,
		y = Markpos.y , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
		rz = Markrot.z, owner = self ,configData = config}  
        

end

function onNotifyCombatAIStateChange(self,msg)
    local State = msg.currentState
	if State == "TETHER" then
	     local ActivityObj = self:GetParentObj().objIDParent
	     ActivityObj:NotifyObject{name = "BossReset" }
	     
	end


end