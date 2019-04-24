--------------------------------------------------------------

-- L_HIDE_OBJECT_CLIENT.lua

-- Generic utility script adding client-side functionality
-- for hiding objects.
-- created abeechler ... 3/8/11

--------------------------------------------------------------

----------------------------------------------
-- Servers tell the client script when and what
-- to hide
----------------------------------------------
function onNotifyClientObject(self,msg)
	-- Split out the timer
	local tTimer = split(msg.name, "_")
	local objectID = tTimer[2] or 0
	local object = GAMEOBJ:GetObjectByID(objectID)
		
	if(tTimer[1] == "HideObject") then
		object:SetVisible{visible = false}
	elseif(tTimer[1] == "UnhideObject") then
		object:SetVisible{visible = true}
	end
end

function split(str, pat)
    local t = {}
    -- Creates a table of strings based on the passed in pattern   
    string.gsub(str .. pat, "(.-)" .. pat, function(result) table.insert(t, result) end)

    return t
end
