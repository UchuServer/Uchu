----------------------------------
-- winter race track falling ice
--
-- created 10/7/10 by mrb... 
----------------------------------
local fxGroup = "falling-ice"
local animID = "interact"

function onCollisionPhantom(self, msg)
    -- find the fx objects
    local fxObj = self:GetObjectsInGroup{group = fxGroup, ignoreSpawners = true}.objects
    -- loop through the fx objects
    for k,animObj in ipairs(fxObj) do
        -- check if it exists
        if animObj:Exists() then
            -- check if it's already playing the animation
            if animObj:GetCurrentAnimation().primaryAnimationID ~= animID then
                --print('play the ' .. animID .. ' anim on: ' .. animObj:GetName().name)
                animObj:PlayAnimation{animationID = animID}
            end
        end
    end
end 