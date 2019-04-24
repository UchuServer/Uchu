--------------------------------------------------------------
-- Description:
--
-- Client script for Guild Master in the FV area
-- Lets client know the object can be interacted with
--
--------------------------------------------------------------


--------------------------------------------------------------
-- Handle this message to override pick type
--------------------------------------------------------------
function onGetOverridePickType(self, msg)

	msg.ePickType = 4
	return msg

end