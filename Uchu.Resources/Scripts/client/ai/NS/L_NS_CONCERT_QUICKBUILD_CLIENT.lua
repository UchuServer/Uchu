--------------------------------------------------------------
-- Client script for the new Concert smash choicebuild, to hide
-- the health bar because it is seen as a problem.
--
-- updated mrb... 11/12/10 - added attachrendereffect
--------------------------------------------------------------

function onRenderComponentReady(self) 
	self:SetNameBillboardState{bState = false }
end

function onScriptNetworkVarUpdate(self, msg)
    for k,v in pairs(msg.tableOfVars) do
        -- start the qb smash fx
        if k == "startEffect" then
			if v > 0 then
				self:AttachRenderEffectFromLua{ effectType = 10, --AttachEffectMsg::RENDER_EFFECTS_CYCLIC_COLOR_GLOW,
												fadeStart = 0.5,
												delta_darken = 0.5,
												fadeEnd = 0.1,
												delta_lighten = 0.05,
												effectTime = v,
												alpha = 0.5,
												color = { r = 1.0, g = 1.0, b = 1.0, a = 1.0 },
												bAffectIcons = false } -- to make the qb smash blink happen.
			else
				self:DetachRenderEffectFromLua{ effectType = 10,
												bAffectIcons = false } -- to stop the qb smash blink.												
			end
		end
    end 
end 