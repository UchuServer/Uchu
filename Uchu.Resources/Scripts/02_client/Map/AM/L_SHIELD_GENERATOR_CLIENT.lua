---------------------------------------------
-- script on the shield generator in CP, displays a popup when the player is close and closes it when the player leaves the area
--
-- created by brandi... 1/6/11
-- updated by brandi... 1/17/11 - added player flag so the popup only appears once a session
---------------------------------------------

-- servers tell the client when a player enters or leaves the volume around the shield
function onNotifyClientObject(self,msg)
	-- server send the player that collided
	local player = msg.paramObj
	--  make sure that player is the local player
	if not player:GetID() == GAMEOBJ:GetLocalCharID() then return end
	-- if the player entered, display the tooltip
	if msg.name == "ENTER" and (player:GetFlag{iFlagID = 121}.bFlag == false) then
		player:DisplayTooltip{ bShow = true, strText = Localize("QB_TOOLTIP_TEAM_BUILD"), id = "TeamBuildTooltip"}
		self:SetVar("ToolTipUp",true)
		player:SetFlag{iFlagID = 121, bFlag = true}
	-- if the player exited or the shield is up, close the tooltip
	elseif msg.name == "LEAVE" then
		if self:GetVar("ToolTipUp") then
			player:DisplayTooltip{ bShow = false, id = "TeamBuildTooltip"}
			self:SetVar("ToolTipUp",false)
		end
	end

end