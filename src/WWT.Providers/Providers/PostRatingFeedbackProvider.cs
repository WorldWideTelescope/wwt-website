using System;

namespace WWT.Providers
{
    public class PostRatingFeedbackProvider : PostRatingFeedback
    {
        public override void Run(WwtContext context)
        {
            context.Response.ClearHeaders();
            context.Response.Clear();
            context.Response.ContentType = "text/xml";
            try
            {
                string query = context.Request.Params["Q"];
                string[] values = query.Split(',');
                string tour = values[0];
                string user = values[1];
                int rating = Convert.ToInt32(values[2]);
                if (rating > -1 && rating < 6)
                {
                    PostFeedback(tour, user, rating);
                }
            }
            catch
            {
            }
            context.Response.End();
        }
    }
}
