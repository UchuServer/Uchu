--------------------------------------------------------------
-- client side Script on the platforms that are on fire in the fire attic of the monastery
-- this script starts out on the client because the server thinks the player collides with the volume even if they dont

-- created by brandi... 6/9/11
--------------------------------------------------------------

function onCollisionPhantom(self,msg)
	local player = msg.objectID 
	-- get the direction the tile

	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "knockback_"..player:GetID(),self )
end

function onTimerDone(self,msg)
	local var = split(msg.name, "_") --Spliting the message name back into the timers name and the player's ID
	local player = ''
		
	if var[2] then
		player = GAMEOBJ:GetObjectByID(var[2]) --Resetting the players Object ID into a Variable
	end

	if var[1] == "knockback" then
		local dir = self:GetObjectDirectionVectors().right 
		-- adjust the velocity
		dir.y = dir.y + 25
		dir.x = dir.x * 15
		dir.z = dir.z * 15
		
		-- send the player off one side
		player:Knockback{vector = dir}
		self:FireEventServerSide{args = "PlayerEntered", senderID = player}
	end
end

----------------------------------------------
-- splits a string based on the pattern passed in
----------------------------------------------
function split(str, pat)
    local t = {}
    -- creates a table of strings based on the passed in pattern   
    string.gsub(str .. pat, "(.-)" .. pat, function(result) table.insert(t, result) end)

    return t
end 