--------------------------------------------------------------
-- Description:
--
-- Client script for Shooting Gallery NPC in ZP area.
-- Lets client know the object can be interacted with
--
--------------------------------------------------------------


--------------------------------------------------------------
-- Handle this message to override pick type
--------------------------------------------------------------
function onGetOverridePickType(self, msg)

	msg.ePickType = 9
	return msg

end