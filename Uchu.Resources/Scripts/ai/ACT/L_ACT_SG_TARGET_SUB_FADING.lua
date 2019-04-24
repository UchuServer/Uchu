local waterheight = 222
local waterlow = 201

function onArrived(self,msg)

    if self:GetPosition().pos.y > waterheight then
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "CheckHeightDown",self )
    elseif self:GetPosition().pos.y < waterheight then
        GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "CheckHeightUp",self )
    end

end

function onTimerDone(self, msg)

	if msg.name == "CheckHeightDown" then
        if self:GetPosition().pos.y > waterheight then

        elseif self:GetPosition().pos.y < waterheight then
            self:PlayFXEffect{effectType = "fade_down"}
        end

    elseif msg.name == "CheckHeightUp" then
        if self:GetPosition().pos.y > waterlow then
            self:PlayFXEffect{effectType = "fade_up"}
        elseif self:GetPosition().pos.y < waterlow then
        end
    end

end