--------------------------------------------------------------
-- Client side script to play the darkitect reveal
-- 
-- created brandi... 11/9/10
--------------------------------------------------------------

--------------------------------------------------------------
-- the server script will tell the client script when the player completes the mission
--------------------------------------------------------------
function onNotifyClientObject(self,msg)
	-- make sure its the right message
	if msg.name == "reveal" then
		-- get the player passed from the client
		local player = msg.paramObj
		-- check player is the local player
		if not player:GetID() == GAMEOBJ:GetLocalCharID() then return end
		
		-- start the darkitect reveal celebration effect
		player:StartCelebrationEffect{rerouteID = player,
                            backgroundObject = 12019,
                            animation = "darkitect-reveal",							
							duration = 24,                           
                            celeLeadIn = .3,
                            celeLeadOut = .5,
                            cameraPathLOT = 12510,
                            pathNodeName = "camera1",
			    soundGUID = "{f75dd0a2-d7bf-4fe7-b68a-54e3a8b54341}",
            		    mixerProgram = "Darkitect_Reveal"}
		-- get the env settings so we can set it back after the reveal; have to set them all separate because we cant up nested tables in setvar
		local envSettings = LEVEL:GetEnvironmentSettings{}
		self:SetVar("specular", envSettings.specular)
		self:SetVar("ambient", envSettings.ambient)
		self:SetVar("directional", envSettings.directional)
		self:SetVar("lightPosition", envSettings.lightPosition)
		self:SetVar("fogColor", envSettings.fogColor)
		self:SetVar("minDrawDistances", envSettings.minDrawDistances)
		self:SetVar("maxDrawDistances", envSettings.maxDrawDistances)
		
		-- change the env settings for the reveal
        LEVEL:ModifyEnvironmentSettings{ambient = {r = .2, g = .2, b = .2},
                                directional = {r = 1, g = 1, b = 1},
                                specular = {r = .4, g = 0, b = 1},
                                lightPosition = {x = -17, y = 8, z = -2.5 },
                                blendTime = .3,
                                fogColor = {r = 0, g = 0, b = 0},                                                                  
                                minDrawDistances = {fogNear = 1000, fogFar = 1000, postFogSolid = 1000, postFogFade = 1000, staticObjectDistance = 1000, dynamicObjectDistance = 1000},
                                maxDrawDistances = {fogNear = 1000, fogFar = 1000, postFogSolid = 1000, postFogFade = 1000, staticObjectDistance = 1000, dynamicObjectDistance = 1000}
                                }       
		
		-- tell the player to tell the script when the celebration is done
		self:SendLuaNotificationRequest{requestTarget = player, messageName = "CelebrationCompleted"}
		
	end
end

--------------------------------------------------------------
-- when the celebration is completed, put the env settings back
--------------------------------------------------------------
function notifyCelebrationCompleted(self,other,msg)
	-- make sure the player is still there
	if not other:Exists() then return end
	-- set the env settings back to what they were
	LEVEL:ModifyEnvironmentSettings{ambient = self:GetVar("ambient"),
                                directional = self:GetVar("directional"),
                                specular = self:GetVar("specular"),
                                lightPosition = self:GetVar("lightPosition"),
                                fogColor = self:GetVar("fogColor"),
                                maxDrawDistances = self:GetVar("maxDrawDistances"),
                                minDrawDistances = self:GetVar("minDrawDistances"),
                                blendTime = 0.1}
	-- cancel the lua request
	self:SendLuaNotificationCancel{requestTarget = other, messageName = "CelebrationCompleted"}
end

