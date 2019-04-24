--------------------------------------------------------------
-- client side Script on the platforms that fall and come back up in the monastery attics
-- 
-- created by brandi... 6/9/11
--------------------------------------------------------------

----------------------------------------------
-- called when network var is updated
----------------------------------------------
function onScriptNetworkVarUpdate(self, msg)
	-- parse through the table of network vars that were updated
    for k,v in pairs(msg.tableOfVars) do
		
        -- start the qb smash fx
        if k == "startEffect" and v then

			self:AttachRenderEffectFromLua{ effectType = 10, --AttachEffectMsg::RENDER_EFFECTS_CYCLIC_COLOR_GLOW,
												fadeStart = 0.5,
												delta_darken = 0.5,
												fadeEnd = 0.1,
												delta_lighten = 0.05,
												--effectTime = v,
												alpha = .25,
												color = { r = 1.0, g = 0.0, b = 0.0, a = 0 },
												bAffectIcons = false } -- to make the qb smash blink happen.
			
			
		elseif k == "stopEffect" and v then
			self:DetachRenderEffectFromLua{ effectType = 10 }-- to make the qb smash blink stop.
				
		end
    end 
end

