﻿using Models.DbModels;

namespace Models.TransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateQuizAttemptResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public UpdateQuizAttemptStatusResult Result { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QuizAttempt Attempt { get; set; }
    }
}
