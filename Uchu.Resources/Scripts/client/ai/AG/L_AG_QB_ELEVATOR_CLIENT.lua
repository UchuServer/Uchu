--------------------------------------------------------------
-- Client-side script for the monument elevators
-- 
-- mrb... 8/24/09 -- added in qb smash fx
--------------------------------------------------------------

function onScriptNetworkVarUpdate(self, msg)
    for k,v in pairs(msg.tableOfVars) do
        -- start the qb smash fx
        if k == "startEffect" and v then
            self:AttachRenderEffectFromLua{ effectType = 10, --AttachEffectMsg::RENDER_EFFECTS_CYCLIC_COLOR_GLOW,
                                            fadeStart = 0.5,
                                            delta_darken = 0.5,
                                            fadeEnd = 0.1,
                                            delta_lighten = 0.05,
                                            effectTime = v,
                                            alpha = 0.5,
                                            color = { r = 1.0, g = 1.0, b = 1.0, a = 1.0 },
                                            bAffectIcons = false } -- to make the qb smash blink happen.
        end
    end 
end