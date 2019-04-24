--------------------------------------------------------------
-- Client side script to display score when an enemy dies
--
-- created by mrb... 1/5/11 added character check
--------------------------------------------------------------
local yOffset = 0 -- vertical offset for the floating text to start at

function onDie(self, msg)    
	if not msg.killerID:Exists() or not msg.killerID:IsCharacter().isChar then return end
	
    local score = self:GetNetworkVar("points") or 0
    
    -- if we have a score display the floating text
    if score > 0 then
		local pos = self:GetPosition().pos
		
		showFloatingText(self, pos, score)
	end
end

function showFloatingText(self, pos, text, yOffset)    
    if not pos or not text then return end
    
    -- get the local player
    local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
    -- if player exists then display the floating text
    if player:Exists() then
        local tTextSize = {x = 0.11, y = 0.16}
        local tTextStart = pos or player:GetPosition().pos
        
        -- offset by 6
        if not yOffset then
            yOffset = 6
        end
        
        tTextStart.y = tTextStart.y + yOffset
		
		-- yellow text
        player:RenderFloatingText{  ni3WorldCoord = tTextStart, ni2ElementSize = tTextSize, 
                                    fFloatAmount = 0.1,  uiTextureHeight = 200, uiTextureWidth = 200,
                                    i64Sender = self, fStartFade = 1.0, 
                                    fTotalFade = 1.25, wsText = text, 
                                    uiFloatSpeed = 4.5, iFontSize = 4, 
                                    niTextColor = {r=255 ,g=255 ,b=255 ,a=0} }
    end     
end 