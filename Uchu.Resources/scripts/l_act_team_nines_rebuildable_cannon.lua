--L_ACT_TEAM_NINES_REBUILDABLE_CANNON.lua
-- Server Side

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('L_ACT_CANNON')

--------------------------------------------------------------
-- Handles the object being destroyed
--------------------------------------------------------------
function onDie(self, msg)

	-- cancel any activity going on
	self:RequestActivityExit{userID = getActivityUser(self), bUserCancel = true}

	-- reset the rebuild
	self:RebuildReset()

end

--------------------------------------------------------------
-- Handles the object changing rebuild states
--------------------------------------------------------------
function onRebuildNotifyState(self, msg)

    -- if someone just finished rebuilding
    if (msg.iState == 3) and (self:IsDead().bDead == true) then

	   -- revive this object so it can be killed again
       self:Resurrect()

	end

end
