using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Models;


namespace Commands
{
    public class PutCommand : ICommand
    {
        public PutCommand(int nextCommandId)
        {
            NextCommandId = nextCommandId;
        }

        public int NextCommandId { get; }

        public int Execute(Board board, Bot bot)
        {
            var putLocation = bot.BoardLocation;
            var putLocationField = board[putLocation];
            if (putLocationField.Piece != null)
            {
                Debug.Log("Successfully put a piece down");
                //putLocationField.Piece = null;
                //bot.Animator.ExecutePut();
            }

            return NextCommandId;
        }
    }
}

