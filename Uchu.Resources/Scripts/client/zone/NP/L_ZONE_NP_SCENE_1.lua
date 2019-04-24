--------------------------------------------------------------
-- Nimbus Park - Scene 1 Specific Client Zone Script Functions
--------------------------------------------------------------

function Scene1Startup(self)

end

--------------------------------------------------------------
-- Called when a Child is loaded
--------------------------------------------------------------
function Scene1OnChildLoaded(self, msg)
	--if msg.templateID == then

	--elseif msg.templateID ==  then
	--end
	for index = 1, #CONSTANTS["SCENE_1_WIZARD_BATTLE_LOTS"] do
		if msg.templateID == CONSTANTS["SCENE_1_WIZARD_BATTLE_LOTS"][index] then
			storeObjectByName(self, "BattleObject", msg.childID)
		end
	end
end


--------------------------------------------------------------
-- Generic notification message
--------------------------------------------------------------
function Scene1OnNotifyObject(self, msg)
	--if msg.name ==  then
	--elseif msg.name ==  then
	--end
end


--------------------------------------------------------------
-- Called when Player Ready from loading into zone
--------------------------------------------------------------
function Scene1OnPlayerReady(self, msg)

end

--------------------------------------------------------------
-- Timers
--------------------------------------------------------------
function Scene1OnTimerDone(self, msg)
	if verifyActors(self) then
		if msg.name == "Wizard1Cast" then
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1.2, "spawnBattleObject",self )
			local obj = getObjectByName(self, "Scene1Wizard1")
			obj:PlayAnimation{animationID = "cast"}
            local num = math.random(1,#CONSTANTS["WIZARD_CAST_TEXT"])
            obj:DisplayChatBubble{wsText = CONSTANTS["WIZARD_CAST_TEXT"][num]}
			GAMEOBJ:GetTimer():AddTimerWithCancel( 7.0, "Wizard1Cast",self )
			GAMEOBJ:GetTimer():AddTimerWithCancel( 3.5, "Wizard2Cast",self )
		elseif msg.name == "Wizard2Cast" then
			GAMEOBJ:GetTimer():AddTimerWithCancel( 1.2, "spawnBattleObject",self )
			local obj = getObjectByName(self, "Scene1Wizard2")
			obj:PlayAnimation{animationID = "cast"}
            local num = math.random(1,#CONSTANTS["WIZARD_CAST_TEXT"])
            obj:DisplayChatBubble{wsText = CONSTANTS["WIZARD_CAST_TEXT"][num]}
		elseif msg.name == "spawnBattleObject" then
			spawnBattleObject(self)
		end
	end
end

function Scene1OnObjectLoaded(self, msg)

end

--------------------------------------------------------------
-- Called when zone object gets an onFireEvent for "SceneActorReady"
-- event.
--------------------------------------------------------------
function Scene1ActorReady(self, actor)
	if actor:GetLOT().objtemplate == CONSTANTS["SCENE_1_WIZARD_1_LOT"] then
		storeObjectByName(self, "Scene1Wizard1", actor)
		GAMEOBJ:GetTimer():AddTimerWithCancel( 5.0, "Wizard1Cast",self )
	elseif actor:GetLOT().objtemplate == CONSTANTS["SCENE_1_WIZARD_2_LOT"] then
		storeObjectByName(self, "Scene1Wizard2", actor)
	end
end

local curBattleIndex = 0
function spawnBattleObject(self)
	if verifyActors(self) then
		if getObjectByName(self, "BattleObject") then
			getObjectByName(self, "BattleObject"):Die{}--killType = "SILENT"}
		end
		
		local newIndex = math.random(#CONSTANTS["SCENE_1_WIZARD_BATTLE_LOTS"])
		while curBattleIndex == newIndex do
			newIndex = math.random(#CONSTANTS["SCENE_1_WIZARD_BATTLE_LOTS"])
		end
		curBattleIndex = newIndex
		local pos = calcSpawnPoint(self)
		local battleLOT = CONSTANTS["SCENE_1_WIZARD_BATTLE_LOTS"][curBattleIndex]
		--print("spawning object: " .. CONSTANTS["SCENE_1_WIZARD_BATTLE_LOTS"][1] .. " at " .. tostring(posMsg.x) .. " " .. tostring(posMsg.y) .. " " .. tostring(posMsg.z))
		--print (battleLOT)
		RESMGR:LoadObject { objectTemplate = battleLOT,
							bIsSmashable = true,
							x = pos.x,
							y = pos.y,
							z = pos.z,
							owner = self }
		else
		print("actors unverified!")
	end
end

function calcSpawnPoint(self)
	local pos1 = getObjectByName(self, "Scene1Wizard1"):GetPosition().pos
	local pos2 = getObjectByName(self, "Scene1Wizard2"):GetPosition().pos
	
	local finalPos = {x = (pos2.x - pos1.x)/2 + pos1.x, y = (pos2.y - pos1.y)/2 + pos1.y, z = (pos2.z - pos1.z)/2 + pos1.z}
	return finalPos
end

function verifyActors(self)
	if getObjectByName(self, "Scene1Wizard1") and getObjectByName(self, "Scene1Wizard2") then
		if getObjectByName(self, "Scene1Wizard1"):Exists() and getObjectByName(self, "Scene1Wizard2"):Exists() then
			return true
		end
	end
	
	if getObjectByName(self, "BattleObject") then
		getObjectByName(self, "BattleObject"):Die{}--killType = "SILENT"}
	end
	return false
end