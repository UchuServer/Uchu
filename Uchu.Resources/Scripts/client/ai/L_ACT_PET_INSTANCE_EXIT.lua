--------------------------------------------------------------
-- Description:
--
-- Client script for Shooting Gallery NPC in GF area.
-- Lets client know the object can be interacted with
--
--------------------------------------------------------------


--------------------------------------------------------------
-- Handle this message to override pick type
--------------------------------------------------------------
function onGetOverridePickType(self, msg)

	msg.ePickType = 14
	return msg

end