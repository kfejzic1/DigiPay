const { defineConfig } = require("cypress");
const nodemailer = require("nodemailer")

const host = process.env.SMTP_HOST || 'localhost'
const port = Number(process.env.SMTP_PORT || 7777)



module.exports = defineConfig({
	projectId: "9hzrgx",
  e2e: {
    setupNodeEvents(on, config) {
      require('cypress-email-results')(on, config, {
		email: ['<email>'],
		emailOnSuccess: true,
		dry: true,
		// pass your transport object
		// transport,
	  })
    },
  },
  //baseUrl: ""
});

