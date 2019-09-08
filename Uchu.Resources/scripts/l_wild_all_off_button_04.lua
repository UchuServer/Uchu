require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')
--[[
function onUse(self, msg)
    local friends = self:GetObjectsInGroup{ group = "drum01" }.objects
        for i = 1, table.maxn (friends) do      
            if friends[i]:GetLOT().objtemplate == 3556 then
            friends[i]:NotifyObject{ name = "stop" }
            end
        end  
end--]]
