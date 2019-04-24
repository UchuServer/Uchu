-- To see how to use the state machine, look at
-- http://abhoth.netdevil.com/display/LUW/Lua+State+Machine

State = {}
State.__index = State

local g_usedEvents = "" -- Used to store all the events that are use by this state machine
-- State constructor
function State.create()
	local newState = {}
	setmetatable(newState, State)
	return newState
end

-- Register all the messages that this state will accept
function State.registerEvents(self)
    local i = next(self,nil)
    while i do
        if i ~= "onEnter" and i ~= "onExit" then
			g_usedEvents = g_usedEvents .. i .. ":"
		end
		i = next(self, i)
    end
end


-- Add a state to an object's state machine.  Takes in the name of the state,
-- and a reference to the game object
-- Param:
-- 		newState - the table that is the state
--		newStateString - the newState in string form. 
--		name - the name of the state in the state machine
--		gameObj - the game object that we are affecting
--      EXAMPLE: AttackAggresive = State.create()
--		addState(AttackAggresive, "AttackAggressive", "Attack", gameObj)
function addState(newState, newStateString, name, gameObj)
	if newState then
		gameObj:AddState { functionName = newStateString, 
						   stateName = name }
		newState:registerEvents()
	else
	    print("--------------SCRIPT ERROR----------------")
		print("Attempting to register invalid state:" .. name .. "func:" .. newStateString )
	end
end


-- Add a sub-state to an object's state machine.  Takes in the name of the state,
-- and a reference to the game object
function addSubState(newState, newStateString, name, gameObj)
	if newState then
		gameObj:AddSubState { functionName = newStateString, 
							  subStateName = name }
		newState:registerEvents()
	else
		print("--------------SCRIPT ERROR----------------")
		print("Attempting to register invalid sub-state" .. name .. "func" .. newStateString )
	end
end

-- Start the state machine at the given inital state
function beginStateMachine( initialState, gameObj )
	if g_usedEvents == "" then
	    print("--------------SCRIPT ERROR----------------")
	    print("Calling beginStateMachine more than once or")
	    print("creating a state machine that accepts no messages")
	
 	else
		gameObj:AddMessage{ messageName = g_usedEvents }
		gameObj:SetState { stateName = initialState }
    	g_usedEvents = "";
	end

end


-- Utility functions so we don't have to remember the variable name needed for SetState
function setState( state, gameObj )
	gameObj:SetState { stateName = state }
end


function setSubState( state, gameObj )
	gameObj:SetSubState { subStateName = state }
end

function resetStateMachineFromError()
    g_usedEvents = "";
end
--------- HOW TO USE STATES ----------------
-- Declare a state by calling State.create() and assigning it
-- to your state.
-- Ex: attackState = State.create()

-- Call HandleEvent.  Every state should define HandleEvent.
-- This is how you do it.
-- attackState = State.create()
-- attackState.HandleEvent = function(msg)
-- if(msg.name == "Hit")
--   DoSomethingAboutIt()
--   return 1
-- else if(msg.name == "Dance")
-- 		...
-- else
--   return 0
-- Only handle the messages you really care about.
-- Return 1 if you handled a message, return 0 if you did not

-- NOTE: you can also define HandleEvent like this:
-- someGenericFunction(msg)
-- ...
-- end
-- attackState.HandleEvent = someGenericFunction

-- You can optionally add the functions Enter and Exit
-- These will be called when a state enters or exits


--- SETTING UP THE STATE MACHINE ---
-- First, create all the states that you will need (see above on how to do that)

-- State Machines are created in onStartup
-- Call self:UseStateMachine{}
-- Then assign the sates as such
-- 	self:AddState { stateName = "stateIdle" }
--	self:AddState { stateName = "stateAttack" }
--		self:AddSubState { stateName = "stateWin" }  -- stateWin is now a substae of stateAttack

--  When you are done adding states, call SetState and pass in the name of the first state you
--  want to start in.
-- 	self:SetState { stateName = "stateIdle" }