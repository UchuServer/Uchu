require('o_mis')


function onFireEventClientSide(self, msg)
--    print ("firer" .. msg.args .. " " .. msg.object:GetID() .. " " .. msg.senderID:GetID() )

	storeObjectByName(self, "player", msg.senderID)
 	storeObjectByName(self, "rocket", msg.object)


    local targetZone = self:GetVar("targetZone")
    local targetScene = self:GetVar("targetScene")
    if targetScene == nil then self:SetVar("targetScene",""); targetScene = "" end
    local playSummary = self:GetVar("playSummary")
    if playSummary == nil then self:SetVar("playSummary",false); playSummary = false end
    local summaryCamera = self:GetVar("summaryCamera")
    if summaryCamera == nil then self:SetVar("summaryCamera",""); summaryCamera = "" end
    local launchPath = self:GetVar("launchCamera")
    if launchPath == nil then self:SetVar("launchCamera",""); launchPath = "" end
    local gmlevel = self:GetVar("GMLevel")
    if gmlevel == nil then gmlevel = 0 ; self:SetVar("GMLevel",0) end

    local playerAnim = self:GetVar("playerAnim")
    local rocketAnim = self:GetVar("rocketAnim")
    if playerAnim == nil then self:SetVar("playerAnim","rocket-launch-AG"); playerAnim = self:GetVar("playerAnim") end
    if rocketAnim == nil then self:SetVar("rocketAnim","launch-AG"); rocketAnim = self:GetVar("rocketAnim") end

    local player=getObjectByName(self,"player")
	local rocket=getObjectByName(self,"rocket")

	if GAMEOBJ:GetLocalCharID() == player:GetID() then
    	UI:SendMessage( "ToggleBackpack", {{"visible", false }, {"tabName", "models"}} )
    end


--    print ("HEAD player " .. self:GetVar("player") )

	local mypos = self:GetPosition().pos 
	local myrot = self:GetRotation()
	
    local config = {
            {"player", "|" .. player:GetID() } ,
            {"rocket", "|" .. rocket:GetID() },
            {"GMLevel", self:GetVar("GMLevel") },
            {"targetZone", self:GetVar("targetZone") },
            {"targetScene", self:GetVar("targetScene") },
            {"summaryCamera", self:GetVar("summaryCamera") },
            {"launchCamera", self:GetVar("launchCamera") },
            {"playSummary", self:GetVar("playSummary") },
            {"playerAnim", self:GetVar("playerAnim") },
            {"rocketAnim", self:GetVar("rocketAnim") },
            {"custom_script_client" , "scripts/client/ai/NEW_ROCKET_LAUNCH_TAIL.lua" } 
       }
		
	RESMGR:LoadObject { objectTemplate = 6 , x = mypos.x , y = mypos.y , z = mypos.z ,owner = self, rw=myrot.w, rx=myrot.x, ry=myrot.y, rz=myrot.z, configData = config }

end
