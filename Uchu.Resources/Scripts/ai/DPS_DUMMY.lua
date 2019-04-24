require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')
function onStartup(self) 
Set = {}
--[[
///////////////////////////////////////////////////////////////////////////
         ____    _  __  ___   _       _       ____  
        / ___|  | |/ / |_ _| | |     | |     / ___| 
        \___ \  | ' /   | |  | |     | |     \___ \ 
         ___) | | . \   | |  | |___  | |___   ___) |
        |____/  |_|\_\ |___| |_____| |_____| |____/                                                                                     
--]]

  
    Set['OverRideHealth']   = false   -- Bool Health Overide
    Set['Health']           = 1       -- Amount of health

    Set['OverRideImag']     = false   -- Bool Imagination Overide
    Set['Imagination']      = nil     -- Amout of Imagination

    Set['OverRideImmunity'] = false   -- Bool Immunity Overide
    Set['Immunity']         = false   -- Bool
    
    Set['OverRideName']     = false
    Set['Name']             = "Master Template" 

    Set['EmoteReact']       = false
    Set['Emote_Delay']      = 2
    Set['React_Set']        = "test"
	
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
    Set['tetherRadius']     = 50     -- Tether  Radius
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
////////////////////////////////////////////////////////////////////////////////
            _       ____    ____   ____     ___  
           / \     / ___|  / ___| |  _ \   / _ \ 
          / _ \   | |  _  | |  _  | |_) | | | | |
         / ___ \  | |_| | | |_| | |  _ <  | |_| |
        /_/   \_\  \____|  \____| |_| \_\  \___/       
        
--]] 

    Set['Aggression']     = "Passive"  -- [Aggressive]--[Neutral]--[Passive]
									   -- [PassiveAggres]-
    Set['AggroNPC']        = false
    Set['AggroDist']      = 3          -- Distance away from target to stop before attacking
    Set['AggroSpeed']     = 2          -- Multiplier of the NPC's base speed to approach while attacking

    -- Aggro Emote
    Set['AggroEmote']      = false     --Plays Emote on Aggro 
    Set['AggroE_Type']     = ""        -- String Name of Emote
    Set['AggroE_Delay']    = 1         -- Animation Delay Time
    
    -- use native code AI?
    Set['SuspendLuaAI']	   = true      -- a state suspending scripted AI
    

--[[

///////////////////////////////////////////////////////////////////////////
         __  __    ___   __     __  _____   __  __   _____   _   _   _____ 
        |  \/  |  / _ \  \ \   / / | ____| |  \/  | | ____| | \ | | |_   _|
        | |\/| | | | | |  \ \ / /  |  _|   | |\/| | |  _|   |  \| |   | |  
        | |  | | | |_| |   \ V /   | |___  | |  | | | |___  | |\  |   | |  
        |_|  |_|  \___/     \_/    |_____| |_|  |_| |_____| |_| \_|   |_|  
--]]

	Set['SuspendLuaMovementAI'] = true -- suspends lua control of movement
    --**********************************************************************
    Set['MovementType']     = "Guard" --["Guard"],["Wander"]
    --**********************************************************************
    -- Attach Way Point Set to NPC -- " this is for NPC's that are not HF placed " 
    Set['WayPointSet']      =  nil
    -- Wander Settings ---------------------------------------------------------
    Set['WanderChance']      = 100          -- Main Weight
    Set['WanderDelayMin']    = 5            -- Min Wander Delay
    Set['WanderDelayMax']    = 5            -- Max Wander Delay
    Set['WanderSpeed']       = 0.5          -- Move speed 
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
--------------------------------------------------------------------------------

    mhealth = self:GetMaxHealth{}.health
	
	time = 0
	timerRunning = false
	
	avgDPS = {}
	avgDPS[1] = nil 
	
end

function onTemplateHit (self, msg ) 

	local dmg = mhealth - self:GetHealth{}.health

	
	SendNetWorkVar( self , "TotalHealth", "" , "", "", "", dmg, "" )
	
	if not timerRunning then
	
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "dpsTimer", self )
		SendNetWorkVar( self , "StateRunning", "" , "", "", "", "", "" )
		timerRunning = true
		
	end

end 

function onUse(self,msg)

	if msg.user then
	
		SendNetWorkVar( self , "pushState", msg.user , self, "", "", "", "" )
		storeObjectByName(self, "player", msg.user)
	end


end


function onMessageBoxRespond(self,msg)


	if msg.identifier == "exit" then
		print "exit"
		-- pop game State
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		SendNetWorkVar( self , "reset", "" , "", "", "", "", "" )
		SendNetWorkVar( self , "popState", "" , "", "", "", "", "" )
	
		
	
	elseif msg.identifier == "stop" then
		print "stop"
		-- Stop the timer
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		self:SetImmunity{immunity = true}
		time = 0

	elseif msg.identifier == "reset" then
	
		-- reset Table
		
		

		for i,v in ipairs(avgDPS) do 
			table.remove(avgDPS,i)
		end
		avgDPS[1] = nil
		-- set immunity
		timeSent = false
		
		self:SetImmunity{immunity = false}
	
		-- Cancel All timers and reset
		
		GAMEOBJ:GetTimer():CancelAllTimers( self )
		timerRunning = false
		time = 0
		
		-- send UI State
	
		SendNetWorkVar( self , "reset", "" , "", "", "", "", "" )
	
		-- reset NPC stats
		
		self:SetHealth{health = mhealth}
		
		
		-- reset PLAYER stats
		local player  = getObjectByName(self, "player")
		
		local pmh = player:GetMaxHealth{}.health
		player:SetHealth{health = pmh } 
		
		local pmi = player:GetMaxImagination{}.imagination
		player:SetImagination{ imagination = pmi}
		
	end
end


function onTemplateTimerDone (self, msg ) 

	if msg.name == "dpsTimer" then
	
		time = time + 1
		timeSent = false
		
		if avgDPS[1] == nil then
			local x =  mhealth - self:GetHealth{}.health
			avgDPS[1] = x
			SendNetWorkVar( self , "Current", "" , "", "", "", x, time )
			timeSent = true
			last =  self:GetHealth{}.health
		else
		
			local x =  last - self:GetHealth{}.health
			if x >= 1 then
				table.insert(avgDPS,(#avgDPS + 1),x)
				
				tbtotal = 0
				for i,v in ipairs(avgDPS) do 
					tbtotal = tbtotal + avgDPS[i]
				end
				local t = roundFloat(tbtotal / #avgDPS,1)
			
				SendNetWorkVar( self , "Current", "" ,  "" , ""..t.."", "nil", x, time )
				timeSent = true

				last =   self:GetHealth{}.health
			end
		end
		
		if not timeSent  then
			SendNetWorkVar( self , "timer", "" ,  "" , "", "", time, "" )
		end
		
		
		
		GAMEOBJ:GetTimer():AddTimerWithCancel( 1 , "dpsTimer", self )
	end

end 

function roundFloat(what, precision)

	   local temp =  math.floor(what*math.pow(10,precision)+0.5) / math.pow(10,precision)
	   local temp2 = string.format("%.2f", temp) 
	   return temp2
end






