--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}
CONSTANTS["PLAYER_START_POS"] = {x = -908.542480, y = 229.773178, z = -7.577438}
CONSTANTS["PLAYER_START_ROT"] = {w = 0.91913521289825, x = 0, y = 0.39394217729568, z = 0}

CONSTANTS["REBUILD_1_TEMPLATEID"] = 2451
CONSTANTS["BOUNCER_TEMPLATE_ID"] = 2687
CONSTANTS["BOUNCER2_TEMPLATE_ID"] = 2975
CONSTANTS["BOX_TEMPLATE_ID"] = 2700
CONSTANTS["BOX2_TEMPLATE_ID"] = 2791
CONSTANTS["WALL_TEMPLATE_ID"] = 2695
CONSTANTS["PLATFORM_TEMPLATE_ID"] = 2700
CONSTANTS["DARKGEN_TEMPLATE_ID"] = 173

CONSTANTS["BOUNCERSTRING1"] = "197.31" .. '\031' .. "695.12" .. '\031' .. "-573.18" .. "#95"
CONSTANTS["BOUNCERSTRING2"] = "198.66" .. '\031' .. "695.13" .. '\031' .. "-582.34" .. "#76"
CONSTANTS["BOUNCERSTRING3"] = "159.29" .. '\031' .. "759.92" .. '\031' .. "-393.84" .. "#55"
CONSTANTS["BOUNCERSTRING4"] = "348.87" .. '\031' .. "721.97" .. '\031' .. "-79.95" .. "#91"
CONSTANTS["BOUNCERSTRING5"] = "393.97" .. '\031' .. "735.42" .. '\031' .. "-115.38" .. "#60"

--TEST
--CONSTANTS = {}

CONSTANTS["CONTROLLER_LOT"] = 2873

CONTROLLER = {}
--0END TEST

--------------------------------------------------------------
-- Zone Variables
--------------------------------------------------------------
--Variables shared throughout zone
local numPlayers = 0
local playerIDs = {}



--##########################################################################################################
-- Helper Functions
--##########################################################################################################

--------------------------------------------------------------
-- store an object by name
--------------------------------------------------------------
function storeObjectByName(self, varName, object)

    idString = object:GetID()
    finalID = "|" .. idString
    self:SetVar(varName, finalID)
   
end


--------------------------------------------------------------
-- get an object by name
--------------------------------------------------------------
function getObjectByName(self, varName)

    targetID = self:GetVar(varName)
    if (targetID) then
		return GAMEOBJ:GetObjectByID(targetID)
	else
		return nil
	end
	
end


--------------------------------------------------------------
-- check to see if a string starts with a substring
--------------------------------------------------------------
function string.starts(String,Start)
   return string.sub(String,1,string.len(Start))==Start
end


--------------------------------------------------------------
-- Increment a saved variable and return its new value
--------------------------------------------------------------
function IncrementVarAndReturn(self,varName)
	local value = self:GetVar(varName)
	if (value) then
		value = value + 1
	end
	self:SetVar(varName,value)
	return value
end



--###############################################################################################################
-- Game Messages
--###############################################################################################################

function onObjectLoaded(self, msg)

	-- controller object loaded
	if (msg.templateID == CONSTANTS["CONTROLLER_LOT"]) then
		CONTROLLER = msg.objectID
	
	end
 

end

--------------------------------------------------------------
-- Startup
--------------------------------------------------------------
function onStartup(self) 
	-- set game state
	self:SetVar("GameStarted", false)
	self:SetVar("GameScore",0)
	self:SetVar("GameTime",0)
	
	print("Zone contol starting up.")
	
	for i=1, 20 do
		self:SetVar("switch" .. tostring(i), false)
	end
	
	self:SetVar("bouncerSpawned", false)
	self:SetVar("currentBox", 0)
	self:SetVar("boxSpawned", false)
	self:SetVar("currentPlatform", 0)
	self:SetVar("platformSpawned", false)
	self:SetVar("BoxCounter", 1)
	self:SetVar("WallCounter", 1)
	self:SetVar("SmashBox1", 0)
	self:SetVar("SmashBox2", 0)
	self:SetVar("SmashBox3", 0)
	self:SetVar("SmashWall1", 0)
	self:SetVar("SmashWall2", 0)
	self:SetVar("CanRespawn456", 0)
	self:SetVar("CanRespawn1011", 0)
	self:SetVar("DarklingsStarted", false)
	
	self:SetVar("currentBouncerCoord", "")

--------------------------------------------------------------
-- spawn initial objects
--------------------------------------------------------------
	RESMGR:LoadObject { objectTemplate = CONSTANTS["BOX2_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    		                    x = 194.72,
    							y = 695.94,
    							z = -473.53,
    		                    owner = self }
	RESMGR:LoadObject { objectTemplate = CONSTANTS["BOX2_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    		                    x = 176.25,
    							y = 694.54,
    							z = -505.29,
    		                    owner = self }
	RESMGR:LoadObject { objectTemplate = CONSTANTS["BOX2_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    		                    x = 155.62,
    		                    y = 695.84,
    		                    z = -471.22,
    		                    owner = self }
    		                    print("Spawning wall")
    RESMGR:LoadObject { objectTemplate = CONSTANTS["WALL_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    		                    x = 145.77,
    		                    y = 755.25,
    		                    z = -234.21,
    		                    rw = 0.0,
    		                    rx = 0.0,
    		                    ry = 0.0,
    		                    rz = 1.0,
    		                    owner = self }
    RESMGR:LoadObject { objectTemplate = CONSTANTS["WALL_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    		                    x = 157.15,
    		                    y = 755.25,
    		                    z = -234.21,
    		                    rw = 0.0,
    		                    rx = 0.0,
    		                    ry = 0.0,
    		                    rz = 1.0,
    		                    owner = self }
end


--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
onTimerDone = function(self, msg)
	if msg.name == "InitBouncer" then
		getObjectByName(self, "currentBouncer"):EmotePlayed{emoteID = self:GetVar("currentBouncerCoord")}
	elseif msg.name == "InitBouncer2" then
		getObjectByName(self, "Bouncer2"):EmotePlayed{emoteID = self:GetVar("Bouncer2Coord")}
	elseif msg.name == "killBox" then
		getObjectByName(self, "currentBox"):Die{killerID = self}
		self:SetVar("boxSpawned", false)
		print("Deleting current Box, ID: " .. self:GetVar("currentBox"))
	elseif msg.name == "killBouncer" then
		print("Deleting current Bouncer, ID: " .. self:GetVar("currentBouncer"))
		getObjectByName(self, "currentBouncer"):Die{killerID = self}
		self:SetVar("bouncerSpawned", false)
	elseif msg.name == "killBouncer2" then
		print("Deleting current Bouncer, ID: " .. self:GetVar("Bouncer2"))
		getObjectByName(self, "Bouncer2"):Die{killerID = self}
		self:SetVar("bouncer2Spawned", false)
	elseif msg.name =="killPlatform" then
		print("Deleting current Platform, ID: " .. self:GetVar("currentPlatform"))
		getObjectByName(self, "currentPlatform"):Die{killerID = self}
		self:SetVar("platformSpawned", false)
	end
end

--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function onChildLoaded(self, msg)
	if msg.templateID == CONSTANTS["BOUNCER_TEMPLATE_ID"] then
		storeObjectByName(self, "currentBouncer", msg.childID)
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "InitBouncer", self )
	elseif msg.templateID == CONSTANTS["BOUNCER2_TEMPLATE_ID"] then
		storeObjectByName(self, "Bouncer2", msg.childID)
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "InitBouncer2", self )
	elseif msg.templateID == CONSTANTS["BOX_TEMPLATE_ID"] then
			storeObjectByName(self, "currentBox", msg.childID)
			print("Box loaded. Spawn state: " .. tostring(self:GetVar("boxSpawned")))
			if not self:GetVar("boxSpawned") then
				print("Box killed before it was loaded!!------------------------- ")
			end
	elseif msg.templateID == CONSTANTS["BOX2_TEMPLATE_ID"] then
		if self:GetVar("BoxCounter") == 1 then
			self:SetVar("BoxCounter", 2)
			storeObjectByName(self, "SmashBox1", msg.childID)
		elseif self:GetVar("BoxCounter") == 2 then
			self:SetVar("BoxCounter", 3)
			storeObjectByName(self, "SmashBox2", msg.childID)
		elseif self:GetVar("BoxCounter") == 3 then
			self:SetVar("BoxCounter", 0)
			storeObjectByName(self, "SmashBox3", msg.childID)
		end
	elseif msg.templateID == CONSTANTS["WALL_TEMPLATE_ID"] then
		if self:GetVar("WallCounter") == 1 then
			self:SetVar("WallCounter", 2)
			storeObjectByName(self, "SmashWall1", msg.childID)
		elseif self:GetVar("WallCounter") == 2 then
			self:SetVar("WallCounter", 0)
			storeObjectByName(self, "SmashWall2", msg.childID)
		end
	elseif msg.templateID == CONSTANTS["PLATFORM_TEMPLATE_ID"] then
		storeObjectByName(self, "currentPlatform", msg.childID)
	elseif msg.templateID == CONSTANTS["DARKGEN_TEMPLATE_ID"] then
		self:SetVar("DarklingsStarted", true)
	end
end

--------------------------------------------------------------
-- Called when Player Loads into Zone
--------------------------------------------------------------
function onPlayerLoaded(self, msg)

	print ("Player Entered: " .. msg.playerID:GetName().name)
	
end

--------------------------------------------------------------
-- Sent from the rebuilds and switches
--------------------------------------------------------------
function onUpdateMissionTask(self, msg)
	-- Handle zone game events
	if string.starts(msg.taskType, "switchon") then
		print(msg.taskType .. " triggered!")
		i, j = string.find(msg.taskType, "switchon")
		local switchID = string.sub(msg.taskType, j+1, j+2)
		self:SetVar("switch" .. switchID, true)
		chooseGroup(self, switchID)
	elseif string.starts(msg.taskType, "switchoff") then
		print(msg.taskType .. " un-triggered!")
		i, j = string.find(msg.taskType, "switchoff")
		local switchID = string.sub(msg.taskType, j+1, j+2)
		self:SetVar("switch" .. switchID, false)
		chooseGroup(self, switchID)
	else
		print("Zone object detected unhandled mission task " .. msg.taskType .. " occured.")
	end
end

function chooseGroup(self, switchID)
	if switchID == "1" then
		print("switch group 1 activated")
		switchEffectsGroup1(self)
	elseif switchID == "2" then
		print("switch group 2 activated")
		switchEffectsGroup2(self)
	elseif switchID == "3" then
		switchEffectsGroup3(self)
	elseif switchID == "4" or switchID == "5" or switchID == "6" then
		print("SwitchID " .. switchID  .. "activated - switch group 456 called")
		switchEffectsGroup456(self)
	elseif switchID == "7" or switchID == "8" or switchID == "9" then
		switchEffectsGroup789(self)
	elseif switchID == "10" or switchID == "11" then
		switchEffectsGroup1011(self)
	elseif switchID == "12" or switchID == "13" or switchID == "14" then
		switchEffectsGroup121314(self)
	elseif switchID == "15" then
		switchEffectsGroup15(self)
	elseif switchID == "16" or switchID == "17" or switchID == "18" then
		switchEffectsGroup161718(self)
	else
		print("WARNING: UNHANDLED SWICTH HIT!!!!!!!!!! switchID = " .. switchID)
	end
end

function switchEffectsGroup1(self)
	if self:GetVar("switch1") then
		if not self:GetVar("boxSpawned") then
    		self:SetVar("boxSpawned", true)
    		print("Box Spawned. Spawn state: " .. tostring(self:GetVar("boxSpawned")))
    		
			-- load the object in the world
    		RESMGR:LoadObject { objectTemplate = CONSTANTS["BOX_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    		                    x = 244.38,
    		                    y = 651.31,
    		                    z = -561.80,
    		                    owner = self }
		end
	else
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "killBox", self )
	end
end

function switchEffectsGroup2(self)
	if self:GetVar("switch2") then
		if not self:GetVar("bouncerSpawned") then
			print("spawning bouncer 2")
    		self:SetVar("currentBouncerCoord", CONSTANTS["BOUNCERSTRING1"])
    		self:SetVar("bouncerSpawned", true)
			-- load the object in the world
    		RESMGR:LoadObject { objectTemplate = CONSTANTS["BOUNCER_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    		                    x = 275,
    		                    y = 652.49,
    		                    z = -519,
    		                    owner = self }
    	end
	else
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "killBouncer", self )
	end
end

function switchEffectsGroup3(self)
	if self:GetVar("switch3") then
		if not self:GetVar("bouncer2Spawned") then
    		self:SetVar("Bouncer2Coord", CONSTANTS["BOUNCERSTRING2"])
    		self:SetVar("bouncer2Spawned", true)
			-- load the object in the world
    		RESMGR:LoadObject { objectTemplate = CONSTANTS["BOUNCER2_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    		                    x = 237.7,
    		                    y = 661.91,
    		                    z = -570.28,
    		                    owner = self }
		end
	else
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "killBouncer2", self )
	end
end

function switchEffectsGroup456(self)

	if self:GetVar("switch4") and self:GetVar("switch5") and self:GetVar("switch6") then 

		self:SetVar("BoxCounter", 1)
		self:SetVar("CanRespawn456", 1)
		
		print("Deleting current Box, ID: " .. self:GetVar("SmashBox1"))
		getObjectByName(self, "SmashBox1"):Die{killerID = self}

		print("Deleting current Box, ID: " .. self:GetVar("SmashBox2"))
		getObjectByName(self, "SmashBox2"):Die{killerID = self}

		print("Deleting current Box, ID: " .. self:GetVar("SmashBox3"))
		getObjectByName(self, "SmashBox3"):Die{killerID = self}


		
	else
		if (self:GetVar("CanRespawn456") == 1) then
		print("creating box")
			self:SetVar("CanRespawn456", 0)
			-- load the object in the world
    		RESMGR:LoadObject { objectTemplate = CONSTANTS["BOX2_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    							x = 194.72,
    							y = 695.94,
    							z = -473.53,
   								owner = self }

			print("creating box")
			-- load the object in the world
 			RESMGR:LoadObject { objectTemplate = CONSTANTS["BOX2_TEMPLATE_ID"], 
								bIsSmashable = true,
   	                    		x = 176.25,
    							y = 694.54,
    							z = -505.29,
    							owner = self }

			print("creating box")
			-- load the object in the world
    		RESMGR:LoadObject { objectTemplate = CONSTANTS["BOX2_TEMPLATE_ID"], 
    		                    bIsSmashable = true,
    		                    x = 155.62,
    		                    y = 695.84,
    		                    z = -471.22,
 								owner = self }
		end
    
	end

end

function switchEffectsGroup789(self)

	if self:GetVar("switch7") and self:GetVar("switch8") and self:GetVar("switch9") then
		if not self:GetVar("bouncerSpawned") then
    		self:SetVar("currentBouncerCoord", CONSTANTS["BOUNCERSTRING3"])
    		self:SetVar("bouncerSpawned", true)
	    	RESMGR:LoadObject { objectTemplate = CONSTANTS["BOUNCER_TEMPLATE_ID"],
    		                    bIsSmashable = true,
    		                    x = 167.7,
    		                    y = 750.59,
    		                    z = -408.28,
    		                    owner = self }
    	end
    elseif getObjectByName(self, "currentBouncer") ~= 0 then
		if self:GetVar("bouncerSpawned") then
			GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "killBouncer", self )
		end
	end
    		

end

function switchEffectsGroup1011(self)
	if self:GetVar("switch10") and self:GetVar("switch11") then 

		self:SetVar("WallCounter", 1)
		self:SetVar("CanRespawn1011", 1)

		print("Deleting current Wall, ID: " .. self:GetVar("SmashWall1"))
		getObjectByName(self, "SmashWall1"):Die{killerID = self}

		print("Deleting current Wall, ID: " .. self:GetVar("SmashWall2"))
		getObjectByName(self, "SmashWall2"):Die{killerID = self}
		
	elseif (self:GetVar("CanRespawn1011") == 1) then
		print("creating walls")
		
		self:SetVar("CanRespawn1011", 0)
		
		-- load the object in the world
		RESMGR:LoadObject { objectTemplate = CONSTANTS["WALL_TEMPLATE_ID"], 
    		                bIsSmashable = true,
    		                x = 145.77,
    		                y = 755.25,
    		                z = -234.21,
    		                rw = 0.0,
    		                rx = 0.0,
    		                ry = 0.0,
    		                rz = 1.0,
    		                owner = self }
		RESMGR:LoadObject { objectTemplate = CONSTANTS["WALL_TEMPLATE_ID"], 
    		                bIsSmashable = true,
    		                x = 157.15,
    		                y = 755.25,
    		                z = -234.21,
    		                rw = 0.0,
    		                rx = 0.0,
    		                ry = 0.0,
    		                rz = 1.0,
    		                owner = self }    
	end
end

function switchEffectsGroup121314(self)
	if self:GetVar("switch12") and self:GetVar("switch13") and self:GetVar("switch14") then
		if not self:GetVar("platformSpawned") then
	    	self:SetVar("platformSpawned", true)
	    	RESMGR:LoadObject { objectTemplate = CONSTANTS["PLATFORM_TEMPLATE_ID"],
    		                    bIsSmashable = true,
    		                    x = 241.96,
    		                    y = 739.43,
    		                    z = -212.13,
    		                    owner = self }
    	end
    elseif getObjectByName(self, "currentPlatform") ~= 0 then
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "killPlatform", self )
	end
    	
end

function switchEffectsGroup15(self)
	if self:GetVar("switch15") then
		if not self:GetVar("bouncerSpawned") then
    		self:SetVar("currentBouncerCoord", CONSTANTS["BOUNCERSTRING4"])
    		self:SetVar("bouncerSpawned", true)
    		RESMGR:LoadObject { objectTemplate = CONSTANTS["BOUNCER_TEMPLATE_ID"],
								bIsSmashable = true,
								x = 276.55,
								y = 743.37,
								z = -212.13,
								owner = self }
		end
    elseif getObjectByName(self, "currentBouncer") ~= 0 then
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "killBouncer", self )
	end
    
end
function switchEffectsGroup161718(self)
	if self:GetVar("switch16") and self:GetVar("switch17") and self:GetVar("switch18") then
		if not self:GetVar("bouncerSpawned") then
    		self:SetVar("currentBouncerCoord", CONSTANTS["BOUNCERSTRING5"])
    		self:SetVar("bouncerSpawned", true)
    		RESMGR:LoadObject { objectTemplate = CONSTANTS["BOUNCER_TEMPLATE_ID"],
								bIsSmashable = true,
								x = 388.58,
								y = 722.52,
								z = -98.23,
								owner = self }
		end
		if not self:GetVar("DarklingsStarted") then
			RESMGR:LoadObject { objectTemplate = CONSTANTS["DARKGEN_TEMPLATE_ID"],
								bIsSmashable = true,
								x = 407.30,
								y = 722.52,
								z = -62.06,
								owner = self }
			RESMGR:LoadObject { objectTemplate = CONSTANTS["DARKGEN_TEMPLATE_ID"],
								bIsSmashable = true,
								x = 384.67,
								y = 722.52,
								z = -74.34,
								owner = self }
			RESMGR:LoadObject { objectTemplate = CONSTANTS["DARKGEN_TEMPLATE_ID"],
								bIsSmashable = true,
								x = 366.31,
								y = 722.52,
								z = -57.13,
								owner = self }
		end
    elseif getObjectByName(self, "currentBouncer") ~= 0 then
		GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "killBouncer", self )
	end
end

function killObj(obj)
	obj:Die{killerID = self}
end
