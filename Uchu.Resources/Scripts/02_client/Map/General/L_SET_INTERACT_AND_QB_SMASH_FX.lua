--------------------------------------------------------------
-- Client-side script for quickbuilds to show their smash effect and make them generically interactive
-- 
-- created brandi... 11/18/10 - combined the generic scripts for both
--------------------------------------------------------------

-- ***********************************************************
-- this will only work on assets that have a server side script 
-- that sets a network var called "startEffect" with a time for that effect to play
-- ***********************************************************

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

function onGetPriorityPickListType(self, msg)
    local myPriority = 0.8
        
    if ( myPriority > msg.fCurrentPickTypePriority ) then

       msg.fCurrentPickTypePriority = myPriority
       msg.ePickType = 14    -- Interactive pick type

    end

    return msg
end 