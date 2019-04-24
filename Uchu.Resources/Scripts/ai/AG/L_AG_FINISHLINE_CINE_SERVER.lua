-- This script is attached to the [server] finish line quickbuild in AG.
-- It handles ending the player's cinematic if the build is cancelled.

function onRebuildCancel(self, msg)
    --print('onRebuildCancel')
    local player = msg.userID
    
    if player:Exists() then
        player:EndCinematic()
    end
end